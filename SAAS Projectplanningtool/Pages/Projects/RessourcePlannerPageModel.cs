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
            if (SelectedEmployeeIds == null || !SelectedEmployeeIds.Any())
            {
                ModelState.AddModelError(string.Empty, "Bitte mindestens einen Mitarbeiter auswählen.");
                return Page();
            }

            var res = await GetOrCreateRessourceAsync(ProjectTaskId);

            // Remove HRG-Data (Mutual exclusivity)
            res.AmountPerHourlyRateGroup = null;

            // Fetch Employees from DB
            var employees = await _context.Employee
                .Where(e => SelectedEmployeeIds.Contains(e.EmployeeId))
                .ToListAsync();

            res.EmployeeRessources = employees;

            res.LatestModificationText = "Einzelmitarbeiter gespeichert";
            res.LatestModificationTimestamp = DateTime.Now;

            if (_context.ProjectTaskRessource.Any(x => x.ProjectTaskRessourceId == res.ProjectTaskRessourceId))
            {
                _context.ProjectTaskRessource.Update(res);
            }
            else
            {
                _context.ProjectTaskRessource.Add(res);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { projectTaskId = ProjectTaskId });
        }

        public async Task<IActionResult> OnPostSaveHRGsAsync()
        {
            if (HRGAmounts == null || !HRGAmounts.Any())
            {
                ModelState.AddModelError(string.Empty, "Bitte mindestens eine Stundenlohngruppe mit Anzahl angeben.");
                return Page();
            }

            var res = await GetOrCreateRessourceAsync(ProjectTaskId);

            // Remove employees (Mutual exclusivity)
            res.EmployeeRessources = null;

            // Build a dictionary with ID & count only (or serialize directly)
            var relevantHRGs = HRGAmounts
                .Where(kv => kv.Key > 0 && kv.Value > 0)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            // Serialize for persistence

            res.LatestModificationText = "HRGs gespeichert";
            res.LatestModificationTimestamp = DateTime.Now;

            if (_context.ProjectTaskRessource.Any(x => x.ProjectTaskRessourceId == res.ProjectTaskRessourceId))
            {
                _context.ProjectTaskRessource.Update(res);
            }
            else
            {
                _context.ProjectTaskRessource.Add(res);
            }

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


            return newRes;
        }
    }

}

