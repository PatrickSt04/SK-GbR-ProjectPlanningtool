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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        [BindProperty] public Customer Customer { get; set; } = default!;
        [BindProperty] public Address Address { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                if (id == null) return NotFound();
                var customer = await _context.Customer
                    .Include(c => c.Address)
                    .FirstOrDefaultAsync(m => m.CustomerId == id);
                if (customer == null) return NotFound();

                Customer = customer;
                Address = Customer.Address ?? new Address();
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR: CustomerManagement/Edit<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid) return Page();

                _context.Address.Update(Address);
                await _context.SaveChangesAsync();

                Customer = await new CustomObjectModifier(_context, _userManager)
                    .AddLatestModificationAsync(User, "Stammdaten geändert", Customer, false);
                _context.Attach(Customer).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Customer.Any(e => e.CustomerId == Customer.CustomerId))
                        return NotFound();
                    throw;
                }

                return RedirectToPage("./Details", new { id = Customer.CustomerId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, Customer, "ERROR: CustomerManagement/Edit<OnPost>") });
            }
        }
    }
}
