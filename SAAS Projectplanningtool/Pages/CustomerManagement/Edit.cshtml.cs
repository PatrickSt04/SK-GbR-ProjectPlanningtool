using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.CustomerManagement
{
    public class EditModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public EditModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(context, userManager);
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;
        [BindProperty]
        public Address Address { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await _logger.Log(null, User, null, "CustomerManagement/Edit<OnGet>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var customer = await _context.Customer
                    .Include(c => c.Address)
                    .FirstOrDefaultAsync(m => m.CustomerId == id);
                if (customer == null)
                {
                    return NotFound();
                }
                Customer = customer;
                Address = Customer.Address;
                await _logger.Log(null, User, null, "CustomerManagement/Edit<OnGet>End" + Customer.CustomerId);
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Customer, "ERROR: CustomerManagement/Edit<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "CustomerManagement/Edit<OnPost>Begin");
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                _context.Address.Update(Address);
                await _context.SaveChangesAsync();
                // der Kunde muss seine Adresse wieder als Navigationselement erhalten
                Customer.Address = Address;
                Customer = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Kunde wurde geändert", Customer, false);
                _context.Attach(Customer).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(Customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                await _logger.Log(null, User, null, "CustomerManagement/Edit<OnPost>End");
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Customer, "ERROR: CustomerManagement/Edit<OnPost>") });
            }
        }

        private bool CustomerExists(string id)
        {
            return _context.Customer.Any(e => e.CustomerId == id);
        }
    }
}
