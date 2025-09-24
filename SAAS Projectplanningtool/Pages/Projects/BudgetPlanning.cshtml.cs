using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class BudgetPlanningModel : BudgetPlannerPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        public BudgetPlanningModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(context, userManager);
        }
        [BindProperty]
        public Project Project { get; set; } = default!;

        [BindProperty]
        public List<HRGAssignment> HRGAmounts { get; set; } = new();


        public class HRGAssignment
        {
            public string HourlyRateGroupId { get; set; } = "";
            public string HourlyRateGroupName { get; set; } = "";
            public decimal HourlyRate { get; set; }
            public int Amount { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await _logger.Log(null, User, null, "Projects.RessourcePlanning<OnGetAsync>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var project = await GetProjectAsync(id);
                if (project == null)
                {
                    return NotFound();
                }
                Project = project;
                AllHourlyRateGroups = await _context.HourlyRateGroup.ToListAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.RessourcePlanning<OnGetAsync>End");
            return Page();
        }

        // Handler für das Speichern der HourlyRateGroups
        public async Task<IActionResult> OnPostSaveHRGs(string? projectId, string? ProjectTaskId)
        {
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(ProjectTaskId))
            {
                return NotFound();
            }

            //if (HRGAmounts == null || !HRGAmounts.Any())
            //{
            //    ModelState.AddModelError(string.Empty, "Bitte mindestens eine Stundensatzgruppe angeben.");
            //    return Page();
            //}

            var task = await GetProjectTaskAsync(ProjectTaskId);
            if (task == null)
            {
                return NotFound();
            }
            // Alte Einträge löschen (oder updaten, je nach Bedarf)
            _context.ProjectTaskHourlyRateGroup.RemoveRange(task.ProjectTaskHourlyRateGroups);

            foreach (var hrg in HRGAmounts.Where(h => h.Amount > 0))
            {
                var hrgEntity = await _context.HourlyRateGroup.FindAsync(hrg.HourlyRateGroupId);
                if (hrgEntity != null)
                {
                    task.ProjectTaskHourlyRateGroups.Add(new ProjectTaskHourlyRateGroup
                    {
                        ProjectTaskId = task.ProjectTaskId,
                        HourlyRateGroupId = hrgEntity.HourlyRateGroupId,
                        Amount = hrg.Amount
                    });
                }
            }

            // letzte Änderung hinzufügen
            task = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Stundensatzgruppen gepflegt", task, false);



            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = projectId });
        }

        public async Task<JsonResult> OnGetReadTaskHRGs(string taskId)
        {
            var hrgs = await _context.ProjectTaskHourlyRateGroup
                .Where(x => x.ProjectTaskId == taskId)
                .Select(x => new
                {
                    hourlyRateGroupId = x.HourlyRateGroupId,
                    hourlyRateGroupName = x.HourlyRateGroup.HourlyRateGroupName,
                    hourlyRate = x.HourlyRateGroup.HourlyRate,
                    amount = x.Amount
                })
            .ToListAsync();

            return new JsonResult(hrgs);
        }

    }

}
