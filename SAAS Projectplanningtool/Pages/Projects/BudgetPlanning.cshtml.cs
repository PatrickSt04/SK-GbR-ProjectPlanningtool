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
        private readonly ProjectStatisticsCalculator _statisticsCalculator;
        public BudgetPlanningModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(context, userManager);
            _statisticsCalculator = new ProjectStatisticsCalculator(context, userManager);
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

        // DTO für Kostenzuweisung
        public class ProjectCostAssignment
        {
            public string? ProjectAdditionalCostsId { get; set; }
            public string AdditionalCostName { get; set; } = "";
            public double AdditionalCostAmount { get; set; }
        }

        [BindProperty]
        public List<ProjectCostAssignment> ProjectCosts { get; set; } = new();


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

        public async Task<IActionResult> OnGetBudgetStatistics(string? projectId)
        {
            if (projectId == null)
            {
                return new JsonResult(null);
            }
            var statistics = await _statisticsCalculator.CalculateBudgetStatisticsAsync(projectId, User);
            return new JsonResult(statistics);
        }



        // Handler zum Speichern der Projektkosten
        public async Task<IActionResult> OnPostSaveProjectCosts(string? projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return NotFound();
            }

            var project = await GetProjectAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            try
            {
                // Alle bestehenden Kosten für das Projekt laden
                var existingCosts = await _context.ProjectAdditionalCosts
                    .Where(c => c.Project.ProjectId == projectId)
                    .ToListAsync();

                var existingCostIds = existingCosts.Select(c => c.ProjectAdditionalCostsId).ToHashSet();
                var submittedCostIds = ProjectCosts
                    .Where(c => !string.IsNullOrEmpty(c.ProjectAdditionalCostsId))
                    .Select(c => c.ProjectAdditionalCostsId)
                    .ToHashSet();

                // Kosten löschen, die nicht mehr in der Submission sind
                var costsToDelete = existingCosts
                    .Where(c => !submittedCostIds.Contains(c.ProjectAdditionalCostsId))
                    .ToList();
                _context.ProjectAdditionalCosts.RemoveRange(costsToDelete);

                // Durchlaufe alle übermittelten Kosten
                foreach (var costDto in ProjectCosts.Where(c => !string.IsNullOrWhiteSpace(c.AdditionalCostName) && c.AdditionalCostAmount > 0))
                {
                    if (!string.IsNullOrEmpty(costDto.ProjectAdditionalCostsId) && existingCostIds.Contains(costDto.ProjectAdditionalCostsId))
                    {
                        // Bestehende Kosten aktualisieren
                        var existingCost = existingCosts.First(c => c.ProjectAdditionalCostsId == costDto.ProjectAdditionalCostsId);
                        existingCost.AdditionalCostName = costDto.AdditionalCostName.Trim();
                        existingCost.AdditionalCostAmount = costDto.AdditionalCostAmount;

                        // Latest Modification aktualisieren
                        existingCost = await new CustomObjectModifier(_context, _userManager)
                            .AddLatestModificationAsync(User, "Projektkosten aktualisiert", existingCost, false);

                        _context.ProjectAdditionalCosts.Update(existingCost);
                    }
                    else
                    {
                        // Neue Kosten erstellen
                        var newCost = new ProjectAdditionalCosts
                        {
                            AdditionalCostName = costDto.AdditionalCostName.Trim(),
                            AdditionalCostAmount = costDto.AdditionalCostAmount,
                            CompanyId = project.CompanyId
                        };

                        // Latest Modification hinzufügen
                        newCost = await new CustomObjectModifier(_context, _userManager)
                            .AddLatestModificationAsync(User, "Projektkosten erstellt", newCost, true);

                        project.ProjectAdditionalCosts.Add(newCost);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToPage(new { id = projectId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
        }

        // Handler zum Laden der Projektkosten
        public async Task<JsonResult> OnGetReadProjectCosts(string projectId)
        {
            var costs = await _context.ProjectAdditionalCosts
                .Include(c => c.LatestModifier)
                .Where(c => c.Project.ProjectId == projectId)
                .Select(c => new
                {
                    projectAdditionalCostsId = c.ProjectAdditionalCostsId,
                    additionalCostName = c.AdditionalCostName,
                    additionalCostAmount = c.AdditionalCostAmount ?? 0,
                    createdTimestamp = c.CreatedTimestamp,
                    latestModificationTimestamp = c.LatestModificationTimestamp,
                    latestModifier = c.LatestModifier != null ? c.LatestModifier.EmployeeDisplayName : null
                })
                .ToListAsync();

            return new JsonResult(costs);
        }


        public async Task<IActionResult> OnPostRecalculateProjectBudgetAsync(string? projectId)
        {
            if (projectId == null)
            {
                return NotFound();
            }
            // Übernimm used budget to project initial budget
            var stats = await _statisticsCalculator.CalculateBudgetStatisticsAsync(projectId, User);
            var usedBudget = stats.UsedBudget;
            var project = await GetProjectAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }
            var projectBudget = project.ProjectBudget;
            if (projectBudget == null)
            {
                return NotFound();
            }
            projectBudget.InitialBudget = usedBudget;

            _context.ProjectBudget.Update(projectBudget);

            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = projectId });
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
