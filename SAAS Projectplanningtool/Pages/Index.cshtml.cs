using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.CustomManagers;
using System.Linq.Expressions;

namespace SAAS_Projectplanningtool.Pages
{
    public class IndexModel : PageModel
    {
        //private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public IList<SAAS_Projectplanningtool.Models.Budgetplanning.Project> projects { get; set; } = new List<SAAS_Projectplanningtool.Models.Budgetplanning.Project>();
        [BindProperty]
        public Company company { get; set; }
        public IndexModel( ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            //_logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Employee employee = default!;
            try
            {
                employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await new Logger(_context, _userManager).Log(ex, User, employee) });
            }

            var query = _context.Project
                .Include(p => p.ProjectBudget)
                .Include(p => p.Company)
                .Include(p => p.Customer)
                .Include(p => p.State)
                .Where(p => p.CompanyId == employee.CompanyId)
                .Where(p => p.IsArchived != true)
                .AsQueryable(); 
            projects = await query.ToListAsync();

            SAAS_Projectplanningtool.CustomManagers.StateManager stateManager = new SAAS_Projectplanningtool.CustomManagers.StateManager(_context);
            foreach (var project in projects)
            {
                project.State = await stateManager.GetProjectState(project.ProjectId);
            }

            company = await _context.Company.FirstOrDefaultAsync(c => c.CompanyId == employee.CompanyId);
            return Page();
        }
    }
}
