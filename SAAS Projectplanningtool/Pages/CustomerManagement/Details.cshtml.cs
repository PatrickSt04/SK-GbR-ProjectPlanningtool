using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.CustomerManagement
{
    public class DetailsModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public DetailsModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public Customer Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .Include(c => c.LatestModifier)
                .Include(c => c.CreatedByEmployee)
                .Include(c => c.Company)
                .Include(c => c.Address)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            else
            {
                Customer = customer;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSetDeleteFlagAsync(string? id)
        {
            try
            {
                await _logger.Log(null, User, null, "/CustomerManagement/Details<OnSetDeleteFlag>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var customer = await _context.Customer.FirstOrDefaultAsync(e => e.CustomerId == id);
                if (customer == null)
                {
                    return NotFound();
                }

                customer = await new CustomObjectModifier(_context, _userManager).SetDeleteFlagAsync(true, customer, User);
                _context.Customer.Update(customer);
                await _context.SaveChangesAsync();

                await _logger.Log(null, User, null, "/CustomerManagement/Details<OnSetDeleteFlag>End");
                return RedirectToPage("/CustomerManagement/Index");
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
                await _logger.Log(null, User, null, "/CustomerManagement/Details<OnUndoDeleteFlag>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var customer = await _context.Customer.FirstOrDefaultAsync(e => e.CustomerId == id);
                if (customer == null)
                {
                    return NotFound();
                }

                customer = await new CustomObjectModifier(_context, _userManager).SetDeleteFlagAsync(false, customer, User);

                _context.Customer.Update(customer);
                await _context.SaveChangesAsync();

                await _logger.Log(null, User, null, "/CustomerManagement/Details<OnUndoDeleteFlag>End");
                return RedirectToPage("/CustomerManagement/Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }
    }
}
