using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.CRM;

namespace SAAS_Projectplanningtool.Pages.CustomerManagement
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public Customer Customer { get; set; } = default!;

        // ─── Load ────────────────────────────────────────────────────────────

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customer
                .Include(c => c.LatestModifier)
                .Include(c => c.CreatedByEmployee)
                .Include(c => c.Company)
                .Include(c => c.Address)
                .Include(c => c.ContactPersons)
                .Include(c => c.CustomerMessages)
                    .ThenInclude(m => m.CreatedByEmployee)
                .Include(c => c.CustomerMessages)
                    .ThenInclude(m => m.ClosedByEmployee)
                .Include(c => c.ContactHistoryEntries)
                    .ThenInclude(e => e.CreatedByEmployee)
                .Include(c => c.MaterialSurcharges)
                .Include(c => c.Projects!)
                    .ThenInclude(p => p.State)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null) return NotFound();
            Customer = customer;
            return Page();
        }

        // ─── Sperren / Entsperren ─────────────────────────────────────────

        public async Task<IActionResult> OnPostSetDeleteFlagAsync(string? id)
        {
            try
            {
                if (id == null) return NotFound();
                var customer = await _context.Customer.FirstOrDefaultAsync(e => e.CustomerId == id);
                if (customer == null) return NotFound();

                customer = await new CustomObjectModifier(_context, _userManager)
                    .SetDeleteFlagAsync(true, customer, User);
                _context.Customer.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        public async Task<IActionResult> OnPostUndoDeleteFlagAsync(string? id)
        {
            try
            {
                if (id == null) return NotFound();
                var customer = await _context.Customer.FirstOrDefaultAsync(e => e.CustomerId == id);
                if (customer == null) return NotFound();

                customer = await new CustomObjectModifier(_context, _userManager)
                    .SetDeleteFlagAsync(false, customer, User);
                _context.Customer.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        // ─── Upgrade Interessent → Kunde ──────────────────────────────────

        public async Task<IActionResult> OnPostUpgradeToKundeAsync(string? id)
        {
            try
            {
                if (id == null) return NotFound();
                var customer = await _context.Customer.FirstOrDefaultAsync(e => e.CustomerId == id);
                if (customer == null) return NotFound();

                customer.CustomerType = CustomerType.Kunde;
                customer = await new CustomObjectModifier(_context, _userManager)
                    .AddLatestModificationAsync(User, "Interessent zu Kunde aufgewertet", customer, false);
                _context.Customer.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        // ─── Ansprechpartner ──────────────────────────────────────────────

        public async Task<IActionResult> OnPostAddContactPersonAsync(
            string id,
            string cpName, string? cpRole, string? cpPhone, string? cpEmail,
            string? cpNotes, bool cpIsPrimary = false)
        {
            try
            {
                var cp = new CustomerContactPerson
                {
                    CustomerId = id,
                    Name = cpName,
                    Role = cpRole,
                    Phone = cpPhone,
                    Email = cpEmail,
                    Notes = cpNotes,
                    IsPrimary = cpIsPrimary
                };
                _context.CustomerContactPerson.Add(cp);

                var customer = await _context.Customer.FindAsync(id);
                if (customer != null)
                {
                    customer = await new CustomObjectModifier(_context, _userManager)
                        .AddLatestModificationAsync(User, $"Ansprechpartner '{cpName}' hinzugefügt", customer, false);
                    _context.Customer.Update(customer);
                }

                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        public async Task<IActionResult> OnPostDeleteContactPersonAsync(string id, string cpId)
        {
            try
            {
                var cp = await _context.CustomerContactPerson.FindAsync(cpId);
                if (cp != null)
                {
                    _context.CustomerContactPerson.Remove(cp);
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        // ─── CustomerMessages / Ticketing ─────────────────────────────────

        public async Task<IActionResult> OnPostAddMessageAsync(
            string id,
            string msgSubject,
            string? msgContent,
            IFormFile? msgAttachment)
        {
            try
            {
                var employee = await new CustomUserManager(_context, _userManager)
                    .GetEmployeeAsync(_userManager.GetUserId(User));

                string? attachmentPath = null;
                string? attachmentFileName = null;

                if (msgAttachment != null && msgAttachment.Length > 0)
                {
                    var uploadsDir = Path.Combine("wwwroot", "uploads", "messages");
                    Directory.CreateDirectory(uploadsDir);
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(msgAttachment.FileName)}";
                    var filePath = Path.Combine(uploadsDir, fileName);
                    using var stream = System.IO.File.Create(filePath);
                    await msgAttachment.CopyToAsync(stream);
                    attachmentPath = fileName;
                    attachmentFileName = msgAttachment.FileName;
                }

                var msg = new CustomerMessage
                {
                    CustomerId = id,
                    Subject = msgSubject,
                    Content = msgContent,
                    Status = MessageStatus.Offen,
                    AttachmentPath = attachmentPath,
                    AttachmentFileName = attachmentFileName,
                    CreatedById = employee?.EmployeeId,
                    CreatedTimestamp = DateTime.Now
                };
                _context.CustomerMessage.Add(msg);
                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        public async Task<IActionResult> OnPostCloseMessageAsync(string id, string msgId)
        {
            try
            {
                var msg = await _context.CustomerMessage.FindAsync(msgId);
                if (msg != null)
                {
                    var employee = await new CustomUserManager(_context, _userManager)
                        .GetEmployeeAsync(_userManager.GetUserId(User));
                    msg.Status = MessageStatus.Geschlossen;
                    msg.ClosedById = employee?.EmployeeId;
                    msg.ClosedTimestamp = DateTime.Now;
                    _context.CustomerMessage.Update(msg);
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        // ─── Kontakt-Historie ─────────────────────────────────────────────

        public async Task<IActionResult> OnPostAddContactHistoryAsync(
            string id,
            int ctType,
            DateTime ctDate,
            string ctNote)
        {
            try
            {
                var employee = await new CustomUserManager(_context, _userManager)
                    .GetEmployeeAsync(_userManager.GetUserId(User));

                var entry = new ContactHistoryEntry
                {
                    CustomerId = id,
                    ContactType = (ContactType)ctType,
                    Note = ctNote,
                    ContactDate = ctDate,
                    CreatedById = employee?.EmployeeId
                };
                _context.ContactHistoryEntry.Add(entry);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Details", null, new { id }, "historie");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        // ─── Materialzuschläge ────────────────────────────────────────────

        public async Task<IActionResult> OnPostAddSurchargeAsync(
            string id,
            string surCategory,
            string? surDescription,
            decimal surPercent)
        {
            try
            {
                var sur = new CustomerMaterialSurcharge
                {
                    CustomerId = id,
                    MaterialCategory = surCategory,
                    Description = surDescription,
                    SurchargePercent = surPercent
                };
                _context.CustomerMaterialSurcharge.Add(sur);
                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        public async Task<IActionResult> OnPostDeleteSurchargeAsync(string id, string surId)
        {
            try
            {
                var sur = await _context.CustomerMaterialSurcharge.FindAsync(surId);
                if (sur != null)
                {
                    _context.CustomerMaterialSurcharge.Remove(sur);
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }
    }
}
