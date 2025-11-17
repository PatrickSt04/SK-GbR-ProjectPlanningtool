using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using Project = SAAS_Projectplanningtool.Models.Budgetplanning.Project;

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
        public List<HRGAssignment> HRGAmounts { get; set; } = new();

        [BindProperty]
        public List<ProjectTask> TaskCatalogTasks { get; set; } = new();

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

        public bool ScheduleAlreadyExists = default!;
        // Nach der HRGAssignment Klasse
        public class InitialHRGPlanning
        {
            public string HourlyRateGroupId { get; set; } = "";
            public string HourlyRateGroupName { get; set; } = "";
            public decimal HourlyRate { get; set; }
            public int Amount { get; set; }
            public double EstimatedHours { get; set; }
        }

        [BindProperty]
        public List<InitialHRGPlanning> InitialHRGPlannings { get; set; } = new();
        // Nach InitialHRGPlanning
        public class InitialAdditionalCost
        {
            public string AdditionalCostName { get; set; } = "";
            public double AdditionalCostAmount { get; set; }
        }

        [BindProperty]
        public List<InitialAdditionalCost> InitialAdditionalCosts { get; set; } = new();

        public class FixCostAssignment
        {
            public string Description { get; set; } = "";
            public double Cost { get; set; }
        }

        [BindProperty]
        public List<FixCostAssignment> FixCosts { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await _logger.Log(null, User, null, "Projects.RessourcePlanning<OnGetAsync>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                await SetProjectBindingAsync(id);
                if (Project == null)
                {
                    return NotFound();
                }
                AllHourlyRateGroups = await _context.HourlyRateGroup.ToListAsync();
                //Wenn im Projekt ein Terminplan vorhanden ist, so ist der wert true (Budgetplanung zeit Planung basierend auf Terminplan an)
                //Wenn noch kein Terminplan vorhanden ist, so wird das initiale Auftragsvolumen geplant (Stundensatzgruppen und Stunden, sowie zusätzliche Aufwendungen)
                ScheduleAlreadyExists = Project.ProjectSections.Any();

                // Initiale HRG-Planung laden, falls vorhanden
                if (Project.ProjectBudget != null && Project.ProjectBudget.InitialHRGPlannings != null)
                {
                    InitialHRGPlannings = Project.ProjectBudget.InitialHRGPlannings
                        .Select(hrg => new InitialHRGPlanning
                        {
                            HourlyRateGroupId = hrg.HourlyRateGroupId,
                            HourlyRateGroupName = hrg.HourlyRateGroupName,
                            HourlyRate = hrg.HourlyRate,
                            Amount = hrg.Amount,
                            EstimatedHours = hrg.EstimatedHours
                        })
                        .ToList();
                }
                else
                {
                    // Leere Liste initialisieren, falls keine Planung vorhanden ist
                    InitialHRGPlannings = new List<InitialHRGPlanning>();
                }
                //Initiale zusätzliche Kosten laden, falls vorhanden
                if (Project.ProjectBudget != null && Project.ProjectBudget.InitialAdditionalCosts != null)
                {
                    InitialAdditionalCosts = Project.ProjectBudget.InitialAdditionalCosts
                        .Select(cost => new InitialAdditionalCost
                        {
                            AdditionalCostName = cost.AdditionalCostName,
                            AdditionalCostAmount = cost.AdditionalCostAmount
                        })
                        .ToList();

                }
                else
                {
                    // Leere Liste initialisieren, falls keine Planung vorhanden ist
                    InitialAdditionalCosts = new List<InitialAdditionalCost>();
                }
                //Wenn der Change Tracker nicht geleert wird, gibt es Probleme mit den Include Befehlen (Beim Laden des Projects werden Includes gefiltert, 
                // wenn nun eine neue DB Abfrage gemacht wird, werden die Includes vom Context in Project aktualisiert

                //konkret: ProjectTasks werden hier geladen, Context ChangeTracker hängt die IDs zum Project in die Navigation sonst an
                //Lösung: ChangeTracker leeren vor neuen Abfragen Optional: 2. Context nutzen
                _context.ChangeTracker.Clear();
                try
                {
                    TaskCatalogTasks = await _context.ProjectTask
                            .Where(pt =>
                                pt.ProjectSection!.ProjectId == id &&
                                pt.IsTaskCatalogEntry &&
                                !pt.IsScheduleEntry
                            )
                            .Include(pt => pt.ProjectTaskFixCosts)
                                .ThenInclude(fc => fc.FixCosts)
                            .ToListAsync();
                }
                catch (Exception)
                {
                    //Bei Neuanlage existiert noch kein PT, dann würde hier eine Exception geworfen werden
                    TaskCatalogTasks = await _context.ProjectTask
                       .Where(pt =>
                           pt.ProjectSection!.ProjectId == id &&
                           pt.IsTaskCatalogEntry &&
                           !pt.IsScheduleEntry
                       )
                       .ToListAsync();
                }

            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.RessourcePlanning<OnGetAsync>End");
            return Page();
        }

        // Handler zum Speichern der initialen Budgetplanung
        public async Task<IActionResult> OnPostSaveInitialBudgetPlanning(string? projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return NotFound();
            }

            await SetProjectBindingAsync(projectId);
            if (Project == null)
            {
                return NotFound();
            }

            try
            {
                // Berechne Gesamtbudget aus HRGs
                decimal totalHRGCosts = 0;
                foreach (var hrg in InitialHRGPlannings.Where(h => h.Amount > 0 && h.EstimatedHours > 0))
                {
                    decimal costPerHour = hrg.HourlyRate * hrg.Amount;
                    decimal totalCost = costPerHour * (decimal)hrg.EstimatedHours;
                    totalHRGCosts += totalCost;
                }

                // Zusätzliche Kosten aus Formular addieren
                decimal totalAdditionalCosts = 0;
                foreach (var cost in InitialAdditionalCosts.Where(c => !string.IsNullOrWhiteSpace(c.AdditionalCostName) && c.AdditionalCostAmount > 0))
                {
                    totalAdditionalCosts += (decimal)cost.AdditionalCostAmount;

                }

                decimal totalBudget = totalHRGCosts + totalAdditionalCosts;

                // ProjectBudget erstellen oder aktualisieren
                var p = await _context.Project
                    .Include(pb => pb.ProjectBudget)
                    .FirstOrDefaultAsync(pb => pb.ProjectId == projectId);
                var projectBudget = p?.ProjectBudget;
                if (projectBudget == null)
                {
                    //Fehlerbehandlung, da ProjectBudget immer nach Projektanlage existieren sollte
                    return NotFound();
                }
                else
                {
                    projectBudget.InitialBudget = (double)totalBudget;
                    //InitialPlannings und InitialAdditionalCosts aktualisieren
                    projectBudget.InitialHRGPlannings = InitialHRGPlannings
                        .Select(hrg => new ProjectBudget.InitialHRGPlanning
                        {
                            HourlyRateGroupId = hrg.HourlyRateGroupId,
                            HourlyRateGroupName = hrg.HourlyRateGroupName,
                            HourlyRate = hrg.HourlyRate,
                            Amount = hrg.Amount,
                            EstimatedHours = hrg.EstimatedHours
                        })
                        .ToList();
                    projectBudget.InitialAdditionalCosts = InitialAdditionalCosts
                        .Select(cost => new ProjectBudget.InitialAdditionalCost
                        {
                            AdditionalCostName = cost.AdditionalCostName,
                            AdditionalCostAmount = cost.AdditionalCostAmount
                        })
                        .ToList();

                    projectBudget = await new CustomObjectModifier(_context, _userManager)
                        .AddLatestModificationAsync(User, "Initiales Budget aktualisiert", projectBudget, false);
                    _context.ProjectBudget.Update(projectBudget);
                }

                await _context.SaveChangesAsync();

                return RedirectToPage("/Projects/Details", new { id = projectId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
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

            await SetProjectBindingAsync(projectId);
            if (Project == null)
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
                            CompanyId = Project.CompanyId
                        };

                        // Latest Modification hinzufügen
                        newCost = await new CustomObjectModifier(_context, _userManager)
                            .AddLatestModificationAsync(User, "Projektkosten erstellt", newCost, true);

                        Project.ProjectAdditionalCosts.Add(newCost);
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
            // Übernimm used budget to project recalculations
            var stats = await _statisticsCalculator.CalculateBudgetStatisticsAsync(projectId, User);
            var usedBudget = stats.UsedBudget;
            await SetProjectBindingAsync(projectId);
            if (Project == null)
            {
                return NotFound();
            }
            var projectBudget = Project.ProjectBudget;
            if (projectBudget == null)
            {
                return NotFound();
            }

            if (projectBudget.BudgetRecalculations == null)
            {
                projectBudget.BudgetRecalculations = new List<BudgetRecalculation>();
            }
            var currentEmployee = await GetEmployeeAsync();

            // Neue Budgetrecalculation hinzufügen
            var newBudgetRecalculation = new BudgetRecalculation
            {
                NewBudget = usedBudget,
                CompanyId = currentEmployee.CompanyId,
                RecalculationDateTime = DateTime.UtcNow,
                RecalculatedBy = currentEmployee
            };
            // Recalculatiion dem Budget anfügen
            projectBudget.BudgetRecalculations.Add(newBudgetRecalculation);
            // DB Updates
            _context.BudgetRecalculation.Add(newBudgetRecalculation);
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

        public async Task<IActionResult> OnPostSaveFixCosts(string? ProjectTaskId, string? ProjectId)
        {
            if (string.IsNullOrEmpty(ProjectTaskId))
            {
                return NotFound();
            }
            var task = await GetProjectTaskAsync(ProjectTaskId);
            if (task == null)
            {
                return NotFound();
            }

            var ptFixCostExisting = await _context.ProjectTaskFixCosts
                .Where(ptfc => ptfc.ProjectTaskFixCostsId == task.ProjectTaskFixCostsId).FirstOrDefaultAsync();
            var updatedFixCosts = FixCosts.Select(bfc => new ProjectTaskFixCosts.FixCost
            {
                Description = bfc.Description,
                Cost = bfc.Cost
            }).ToList();
            if (ptFixCostExisting != null)
            {

                ptFixCostExisting.FixCosts = updatedFixCosts;
                _context.ProjectTaskFixCosts.Update(ptFixCostExisting);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new {id = ProjectId});


        }
    }
}
