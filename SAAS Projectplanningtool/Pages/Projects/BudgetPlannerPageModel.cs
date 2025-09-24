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


        //[BindProperty]
        //public string ProjectTaskId { get; set; } = "";



        public List<HourlyRateGroup> AllHourlyRateGroups { get; set; } = new List<HourlyRateGroup>();


        //[BindProperty]
        //public Dictionary<int, HourlyRateGroup> AmountPerHourlyRateGroup { get; set; } = new Dictionary<int, HourlyRateGroup>();



        protected async Task<ProjectTask> GetProjectTaskAsync(string projectTaskId)
        {
            var existing = await _context.ProjectTask
                .Include(pt => pt.ProjectTaskHourlyRateGroups)
                .FirstOrDefaultAsync(x => x.ProjectTaskId == projectTaskId);
            return existing;
        }

    }
}