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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
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
                await _logger.Log(null, User, null, "Customer/Index<OnGet>Begin");
                var employee = await new CustomUserManager(_context, _userManager)
                    .GetEmployeeAsync(_userManager.GetUserId(User));

                Customer = await _context.Customer
                    .Include(c => c.Address)
                    .Include(c => c.Company)
                    .Include(c => c.CreatedByEmployee)
                    .Include(c => c.LatestModifier)
                    .Where(c => c.CompanyId == employee.CompanyId)
                    .OrderBy(c => c.CustomerName)
                    .ToListAsync();

                await _logger.Log(null, User, null, "Customer/Index<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, "ERROR:Customer/Index<OnGet>") });
            }
        }
    }
}
