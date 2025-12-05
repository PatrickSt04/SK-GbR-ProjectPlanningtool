using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class BudgetPlannerPageModel : ProjectPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        //Diese Klasse wird in den Razor Pages verwendet, um im Terminplan Ressourcen einzubinden.
        public BudgetPlannerPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }
        public List<HourlyRateGroup> AllHourlyRateGroups { get; set; } = new List<HourlyRateGroup>();
        protected async Task<ProjectTask?> GetProjectTaskAsync(string projectTaskId)
        {
            var existing = await _context.ProjectTask
                .Include(pt => pt.ProjectTaskHourlyRateGroups)
                .Include(pt => pt.State)
                .FirstOrDefaultAsync(x => x.ProjectTaskId == projectTaskId);
            return existing;
        }
        protected async Task<ProjectTaskCatalogTask?> GetTaskCatalogTaskAsync(string projectTaskId)
        {
            var existing = await _context.ProjectTaskCatalogTask
                .Include(pt => pt.ProjectTaskFixCosts)
                .Include(pt => pt.State)
                .FirstOrDefaultAsync(x => x.ProjectTaskCatalogTaskId == projectTaskId);
            return existing;
        }

    }
}