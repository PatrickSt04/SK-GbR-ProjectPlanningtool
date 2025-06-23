using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.RessourcePlanning;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class RessourcePlannerPageModel : ProjectPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        //Diese Klasse wird in den Razor Pages verwendet, um im Terminplan Ressourcen einzubinden.
        public RessourcePlannerPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }


        [BindProperty]
        public string ProjectTaskId { get; set; }

        [BindProperty]
        public List<string> SelectedEmployeeIds { get; set; }

        [BindProperty]
        public Dictionary<int, int> HRGAmounts { get; set; } = new();

        public List<Employee> AllEmployees { get; set; }
        public List<HourlyRateGroup> AllHourlyRateGroups { get; set; }

        public async Task<IActionResult> OnPostSaveEmployeesAsync()
        {
            var res = await GetOrCreateRessourceAsync(ProjectTaskId);

            // Clear HRG if present
            res.AmountPerHourlyRateGroup = null;

            //// Set new employees
            //res.EmployeeRessources = SelectedEmployeeIds
            //    .Select(id => new Employee { EmployeeId = id }) // ggf. nur ID setzen
            //    .ToList();

            res.LatestModificationText = "Einzelmitarbeiter gespeichert";
            res.LatestModificationTimestamp = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToPage(new { projectTaskId = ProjectTaskId });
        }

        public async Task<IActionResult> OnPostSaveHRGsAsync()
        {
            var res = await GetOrCreateRessourceAsync(ProjectTaskId);

            // Clear employees
            res.EmployeeRessources = null;

            // Set new HRGs
            res.AmountPerHourlyRateGroup = (Dictionary<int, HourlyRateGroup>?)HRGAmounts
                .Where(kv => kv.Key > 0);

            res.LatestModificationText = "HRGs gespeichert";
            res.LatestModificationTimestamp = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToPage(new { projectTaskId = ProjectTaskId });
        }

        private async Task<ProjectTaskRessource> GetOrCreateRessourceAsync(string projectTaskId)
        {
            var existing = await _context.ProjectTaskRessource.Include(t => t.EmployeeRessources)
                .FirstOrDefaultAsync(x => x.ProjectTaskId == projectTaskId);

            if (existing != null) return existing;

            var newRes = new ProjectTaskRessource
            {
                ProjectTaskId = projectTaskId,
                CreatedTimestamp = DateTime.Now,
            };

            _context.ProjectTaskRessource.Add(newRes);
            return newRes;
        }
    }

}

