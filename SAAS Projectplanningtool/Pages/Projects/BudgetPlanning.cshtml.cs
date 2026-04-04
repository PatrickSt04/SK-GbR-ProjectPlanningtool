using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.Models.TimeTracking;
using Project = SAAS_Projectplanningtool.Models.Budgetplanning.Project;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class BudgetPlanningModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        : BudgetPlannerPageModel(context, userManager, roleManager)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly Logger _logger = new(context, userManager);
        private readonly ProjectStatisticsCalculator _statisticsCalculator = new(context, userManager);
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        // DTO f�r zus�tzliche Projektkosten
        public class ProjectCostAssignment
        {
            public string? ProjectAdditionalCostsId { get; set; }
            public string AdditionalCostName { get; set; } = "";
            public double AdditionalCostAmount { get; set; }
        }

        [BindProperty]
        public List<ProjectCostAssignment> ProjectCosts { get; set; } = new();

        public bool ScheduleAlreadyExists = default!;

        // Initiale Budgetplanung (vor Terminplan)
        public class InitialHRGPlanning
        {
            public string ProjectHRGId { get; set; } = "";
            public string ProjectHRGName { get; set; } = "";
            public decimal HourlyRate { get; set; }
            public int Amount { get; set; }
            public double EstimatedHours { get; set; }
        }

        [BindProperty]
        public List<InitialHRGPlanning> InitialHRGPlannings { get; set; } = new();

        public class InitialAdditionalCost
        {
            public string AdditionalCostName { get; set; } = "";
            public double AdditionalCostAmount { get; set; }
        }

        [BindProperty]
        public List<InitialAdditionalCost> InitialAdditionalCosts { get; set; } = new();


        #region Time Tracking Properties
        public List<TimeEntry> TimeEntries { get; set; } = new();
        public bool IsWorkerRole { get; set; } = false;
        public string? CurrentEmployeeId { get; set; }
        public List<Employee> CompanyEmployees { get; set; } = new();
        public double TotalProjectHours { get; set; }
        public int TotalTimeEntryCount { get; set; }
        [BindProperty(SupportsGet = true)]
        public int TimeTrackingPage { get; set; } = 1;
        public int TimeTrackingTotalPages { get; set; }
        private const int TimeTrackingPageSize = 15;
        #endregion
        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {

                await _logger.Log(null, User, null, "Projects.BudgetPlanning<OnGetAsync>Begin");
                if (id == null)
                    return NotFound();

                await SetProjectBindingAsync(id);
                if (Project == null)
                    return NotFound();

                var employee = await GetEmployeeAsync();
                if (employee == null || employee.CompanyId == null)
                    return NotFound();

                AllHourlyRateGroups = await _context.ProjectHourlyRateGroup
                   .Include(h => h.HourlyRateGroup)
                   .Where(p => p.HourlyRateGroup != null && !p.HourlyRateGroup.DeleteFlag)
                   .Where(p => p.CompanyId == employee.CompanyId)
                   .ToListAsync();
                ScheduleAlreadyExists = Project.ProjectSections.Any();

                // Initiale HRG-Planung laden
                InitialHRGPlannings = Project.ProjectBudget?.InitialHRGPlannings?
                    .Select(hrg => new InitialHRGPlanning
                    {
                        ProjectHRGId = hrg.ProjectHRGId,
                        ProjectHRGName = hrg.ProjectHRGName,
                        HourlyRate = hrg.HourlyRate,
                        Amount = hrg.Amount,
                        EstimatedHours = hrg.EstimatedHours
                    })
                    .ToList() ?? new List<InitialHRGPlanning>();

                // Initiale Zusatzkosten laden
                InitialAdditionalCosts = Project.ProjectBudget?.InitialAdditionalCosts?
                    .Select(cost => new InitialAdditionalCost
                    {
                        AdditionalCostName = cost.AdditionalCostName,
                        AdditionalCostAmount = cost.AdditionalCostAmount
                    })
                    .ToList() ?? new List<InitialAdditionalCost>();


                #region Time Tracking: Mitarbeiter- und Zeiteintr�ge laden
                employee = await GetEmployeeAsync();
                if (employee != null && employee.CompanyId != null)
                {

                    CurrentEmployeeId = employee?.EmployeeId;

                    // Rolle ermitteln

                    IsWorkerRole = User.IsInRole("Viewer");

                    // Mitarbeiter-Liste laden (nur f�r Nicht-Worker, f�r das Dropdown)
                    if (!IsWorkerRole && employee?.CompanyId != null)
                    {
                        CompanyEmployees = await _context.Employee
                            .Where(e => e.CompanyId == employee.CompanyId)
                            .Where(e => e.DeleteFlag == false)
                            .OrderBy(e => e.EmployeeDisplayName)
                            .ToListAsync();
                    }

                    // Zeiteintr�ge mit Pagination laden
                    if (TimeTrackingPage < 1) TimeTrackingPage = 1;

                    var timeEntryQuery = _context.TimeEntry
                        .Include(t => t.Employee)
                        .Where(t => t.ProjectId == id)
                        .Where(t => t.CompanyId == employee.CompanyId)
                        .OrderByDescending(t => t.WorkDate)
                        .ThenByDescending(t => t.StartTime);

                    TotalTimeEntryCount = await timeEntryQuery.CountAsync();
                    TimeTrackingTotalPages = (int)Math.Ceiling(TotalTimeEntryCount / (double)TimeTrackingPageSize);

                    TimeEntries = await timeEntryQuery
                        .Skip((TimeTrackingPage - 1) * TimeTrackingPageSize)
                        .Take(TimeTrackingPageSize)
                        .ToListAsync();

                    // Gesamtstunden berechnen (�ber ALLE Eintr�ge, nicht nur aktuelle Seite)
                    var allTimeEntries = await _context.TimeEntry
                        .Where(t => t.ProjectId == id)
                        .Where(t => t.CompanyId == employee.CompanyId)
                        .ToListAsync();

                    TotalProjectHours = allTimeEntries.Sum(t => t.NetWorkingHours);

                    #endregion

                }
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }

            await _logger.Log(null, User, null, "Projects.BudgetPlanning<OnGetAsync>End");
            return Page();
        }

        // Initiale Budgetplanung speichern
        public async Task<IActionResult> OnPostSaveInitialBudgetPlanning(string? projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return NotFound();

            await SetProjectBindingAsync(projectId);
            if (Project == null)
                return NotFound();

            try
            {
                decimal totalHRGCosts = 0;
                foreach (var hrg in InitialHRGPlannings.Where(h => h.Amount > 0 && h.EstimatedHours > 0))
                {
                    totalHRGCosts += hrg.HourlyRate * hrg.Amount * (decimal)hrg.EstimatedHours;
                }

                decimal totalAdditionalCosts = InitialAdditionalCosts
                    .Where(c => !string.IsNullOrWhiteSpace(c.AdditionalCostName) && c.AdditionalCostAmount > 0)
                    .Sum(c => (decimal)c.AdditionalCostAmount);

                var p = await _context.Project
                    .Include(pb => pb.ProjectBudget)
                    .FirstOrDefaultAsync(pb => pb.ProjectId == projectId);

                var projectBudget = p?.ProjectBudget;
                if (projectBudget == null)
                    return NotFound();

                projectBudget.InitialBudget = (double)(totalHRGCosts + totalAdditionalCosts);
                projectBudget.InitialHRGPlannings = InitialHRGPlannings
                    .Select(hrg => new ProjectBudget.InitialHRGPlanning
                    {
                        ProjectHRGId = hrg.ProjectHRGId,
                        ProjectHRGName = hrg.ProjectHRGName,
                        HourlyRate = hrg.HourlyRate,
                        Amount = hrg.Amount,
                        EstimatedHours = hrg.EstimatedHours
                    }).ToList();

                projectBudget.InitialAdditionalCosts = InitialAdditionalCosts
                    .Select(cost => new ProjectBudget.InitialAdditionalCost
                    {
                        AdditionalCostName = cost.AdditionalCostName,
                        AdditionalCostAmount = cost.AdditionalCostAmount
                    }).ToList();

                projectBudget = await new CustomObjectModifier(_context, _userManager)
                    .AddLatestModificationAsync(User, "Initiales Budget aktualisiert", projectBudget, false);

                _context.ProjectBudget.Update(projectBudget);
                await _context.SaveChangesAsync();

                return RedirectToPage("/Projects/Details", new { id = projectId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
        }

        // Budget-Statistiken laden (basiert jetzt auf Zeitbuchungen)
        public async Task<IActionResult> OnGetBudgetStatistics(string? projectId)
        {
            if (projectId == null)
                return new JsonResult(null);

            var statistics = await _statisticsCalculator.CalculateBudgetStatisticsAsync(projectId, User);
            return new JsonResult(statistics);
        }


        public async Task<JsonResult> OnGetTimeTrackingBreakdown(string projectId)
        {
            var breakdown = await _statisticsCalculator.CalculateTimeTrackingBreakdownAsync(projectId);
            return new JsonResult(breakdown);
        }
        // Nachkalkulation ansto�en
        public async Task<IActionResult> OnPostRecalculateProjectBudgetAsync(string? projectId)
        {
            if (projectId == null)
                return NotFound();

            var stats = await _statisticsCalculator.CalculateBudgetStatisticsAsync(projectId, User);
            var usedBudget = stats.UsedBudget;

            await SetProjectBindingAsync(projectId);
            if (Project == null)
                return NotFound();

            if (Project.ProjectBudget == null)
                return NotFound();

            Project.ProjectBudget.BudgetRecalculations ??= new List<BudgetRecalculation>();

            var currentEmployee = await GetEmployeeAsync();
            var newBudgetRecalculation = new BudgetRecalculation
            {
                NewBudget = usedBudget,
                CompanyId = currentEmployee.CompanyId,
                RecalculationDateTime = DateTime.UtcNow,
                RecalculatedBy = currentEmployee
            };

            _context.BudgetRecalculation.Add(newBudgetRecalculation);
            Project.ProjectBudget.BudgetRecalculations.Add(newBudgetRecalculation);
            _context.Update(Project);

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id = projectId });
        }

        // Zus�tzliche Projektkosten speichern
        public async Task<IActionResult> OnPostSaveProjectCosts(string? projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return NotFound();

            await SetProjectBindingAsync(projectId);
            if (Project == null)
                return NotFound();

            try
            {
                var existingCosts = await _context.ProjectAdditionalCosts
                    .Where(c => c.Project.ProjectId == projectId)
                    .ToListAsync();

                var existingCostIds = existingCosts.Select(c => c.ProjectAdditionalCostsId).ToHashSet();
                var submittedCostIds = ProjectCosts
                    .Where(c => !string.IsNullOrEmpty(c.ProjectAdditionalCostsId))
                    .Select(c => c.ProjectAdditionalCostsId)
                    .ToHashSet();

                var costsToDelete = existingCosts
                    .Where(c => !submittedCostIds.Contains(c.ProjectAdditionalCostsId))
                    .ToList();
                _context.ProjectAdditionalCosts.RemoveRange(costsToDelete);

                foreach (var costDto in ProjectCosts.Where(c => !string.IsNullOrWhiteSpace(c.AdditionalCostName) && c.AdditionalCostAmount > 0))
                {
                    if (!string.IsNullOrEmpty(costDto.ProjectAdditionalCostsId) && existingCostIds.Contains(costDto.ProjectAdditionalCostsId))
                    {
                        var existingCost = existingCosts.First(c => c.ProjectAdditionalCostsId == costDto.ProjectAdditionalCostsId);
                        existingCost.AdditionalCostName = costDto.AdditionalCostName.Trim();
                        existingCost.AdditionalCostAmount = costDto.AdditionalCostAmount;
                        existingCost = await new CustomObjectModifier(_context, _userManager)
                            .AddLatestModificationAsync(User, "Projektkosten aktualisiert", existingCost, false);
                        _context.ProjectAdditionalCosts.Update(existingCost);
                    }
                    else
                    {
                        var newCost = new ProjectAdditionalCosts
                        {
                            AdditionalCostName = costDto.AdditionalCostName.Trim(),
                            AdditionalCostAmount = costDto.AdditionalCostAmount,
                            CompanyId = Project.CompanyId
                        };
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

        // Projektkosten lesen
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
        #region Time Tracking Handlers
        public async Task<IActionResult> OnPostCreateTimeEntryAsync(
            string projectId,
            string? employeeId,
            DateOnly workDate,
            TimeOnly startTime,
            TimeOnly endTime,
            int breakMinutes,
            string? description)
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/Details<OnPostCreateTimeEntryAsync>Begin");

                var currentEmployee = await new CustomUserManager(_context, _userManager)
                    .GetEmployeeAsync(_userManager.GetUserId(User));

                if (currentEmployee == null) return NotFound();

                // Rolle pr�fen
                bool isWorker = User.IsInRole("Viewer");

                // Ziel-Mitarbeiter bestimmen
                string targetEmployeeId;
                if (isWorker)
                {
                    // Worker d�rfen nur f�r sich selbst erfassen
                    targetEmployeeId = currentEmployee.EmployeeId;
                }
                else
                {
                    // Andere Rollen: ausgew�hlten Mitarbeiter oder sich selbst
                    targetEmployeeId = employeeId ?? currentEmployee.EmployeeId;
                }

                // Validierung: Endzeit > Startzeit
                if (endTime <= startTime)
                {
                    TempData.SetMessage("Error", "Endzeit muss nach der Startzeit liegen.");
                    return RedirectToPage(new { id = projectId });
                }

                // Validierung: Netto-Arbeitszeit > 0
                var totalMinutes = (endTime - startTime).TotalMinutes - breakMinutes;
                if (totalMinutes <= 0)
                {
                    TempData.SetMessage("Error", "Die Netto-Arbeitszeit muss positiv sein.");
                    return RedirectToPage(new { id = projectId });
                }

                // TimeEntry erstellen
                var timeEntry = new TimeEntry
                {
                    CompanyId = currentEmployee.CompanyId,
                    ProjectId = projectId,
                    EmployeeId = targetEmployeeId,
                    WorkDate = workDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    BreakMinutes = breakMinutes,
                    Description = description
                };

                // Audit-Felder setzen (Created + LatestModification)
                timeEntry = await new CustomObjectModifier(_context, _userManager)
                    .AddLatestModificationAsync(User, "Zeiteintrag erstellt", timeEntry, true);

                _context.TimeEntry.Add(timeEntry);
                await _context.SaveChangesAsync();

                TempData.SetMessage("Success", "Zeiteintrag erfolgreich gespeichert.");
                await _logger.Log(null, User, null, "Projects/Details<OnPostCreateTimeEntryAsync>End");

                return RedirectToPage(new { id = projectId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "Projects/Details<OnPostCreateTimeEntryAsync>Error") });
            }
        }

        /// <summary>
        /// Zeiteintrag l�schen.
        /// Worker: nur eigene Eintr�ge. Andere Rollen: alle Eintr�ge.
        /// </summary>
        public async Task<IActionResult> OnPostDeleteTimeEntryAsync(
            string timeEntryId,
            string projectId,
            int timeTrackingPage = 1)
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/Details<OnPostDeleteTimeEntryAsync>Begin");

                var currentEmployee = await new CustomUserManager(_context, _userManager)
                    .GetEmployeeAsync(_userManager.GetUserId(User));

                if (currentEmployee == null) return NotFound();

                var entry = await _context.TimeEntry.FirstOrDefaultAsync(t =>
                    t.TimeEntryId == timeEntryId &&
                    t.CompanyId == currentEmployee.CompanyId);

                if (entry == null) return NotFound();

                if (User.IsInRole("Viewer") && entry.EmployeeId != currentEmployee.EmployeeId)
                {
                    TempData.SetMessage("Error", "Sie k�nnen nur eigene Zeiteintr�ge l�schen.");
                    return RedirectToPage(new { id = projectId, timeTrackingPage });
                }

                _context.TimeEntry.Remove(entry);
                await _context.SaveChangesAsync();

                TempData.SetMessage("Success", "Zeiteintrag wurde gel�scht.");
                await _logger.Log(null, User, null, "Projects/Details<OnPostDeleteTimeEntryAsync>End");

                return RedirectToPage(new { id = projectId, timeTrackingPage });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "Projects/Details<OnPostDeleteTimeEntryAsync>Error") });
            }
        }
    }

    #endregion

}