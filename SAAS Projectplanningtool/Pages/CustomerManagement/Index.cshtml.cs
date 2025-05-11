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
    public class IndexModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly Logger _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public IList<Customer> Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "Customer/Index<OnGet>Beginn");
                var employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                Customer = await _context.Customer
                    .Include(c => c.Address)
                    .Include(c => c.Company)
                    .Where(c => c.CompanyId == employee.CompanyId)
                    .Include(c => c.CreatedByEmployee)
                    .Include(c => c.Address)
                    .Include(c => c.LatestModifier).ToListAsync();
                await _logger.Log(null, User, null, "Customer/Index<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, "ERROR:Customer/Index<OnGet>End") });
            }
        }
    }
}
