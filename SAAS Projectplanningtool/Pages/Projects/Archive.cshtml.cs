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
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class IndexModel : ProjectPageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;    
        private readonly Logger _logger;

        public IndexModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(context, userManager);
        }

        public IList<Project> Projects { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            
            try
            {
                var employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                await _logger.Log(null, User, null, "Projects.Archive<OnGetAsync>Begin");
                Projects = await _context.Project
                    .Include(p => p.Company)
                    .Include(p => p.Customer)
                    .Include(p => p.LatestModifier)
                    .Include(p => p.ProjectBudget)
                    .Include(p => p.State)
                    .Where(p => p.IsArchived == true)
                    .Where(p => p.CompanyId == employee.CompanyId)
                    .ToListAsync();
                await _logger.Log(null, User, null, "Projects.Archive<OnGetAsync>End");
            }
            catch (Exception ex)
            {
               return RedirectToPage("/Error", new {id = await _logger.Log(ex, User, null, "Projects.Archive<OnGetAsync>Error") });
            }
            return Page();
        }
    }
}
