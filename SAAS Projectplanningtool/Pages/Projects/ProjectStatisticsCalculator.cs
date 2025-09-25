using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class ProjectStatisticsCalculator
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjectStatisticsCalculator(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Berechnet alle relevanten Statistik-Werte für ein Projekt.
        /// </summary>
        public async Task<ProjectStatistics> CalculateStatisticsAsync(string projectId, ClaimsPrincipal user)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                throw new ArgumentException("Projekt-ID darf nicht leer sein.", nameof(projectId));

            var employee = await GetEmployeeAsync(user);
            var sectionIds = await GetProjectSectionIdsAsync(projectId, employee.CompanyId);
            var completedStateId = await GetCompletedStateIdAsync();

            int totalTasks = await GetTaskCountAsync(sectionIds, employee.CompanyId);
            int completedTasks = await GetTaskCountAsync(sectionIds, employee.CompanyId, completedStateId);

            return new ProjectStatistics(totalTasks, completedTasks);
        }

        /// <summary>
        /// Berechnet detaillierte Budget-Statistiken für ein Projekt
        /// </summary>
        public async Task<ProjectBudgetStatistics> CalculateBudgetStatisticsAsync(string projectId, ClaimsPrincipal user)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                throw new ArgumentException("Projekt-ID darf nicht leer sein.", nameof(projectId));

            var employee = await GetEmployeeAsync(user);

            // Lade das vollständige Projekt mit allen Abhängigkeiten
            var project = await GetFullProjectAsync(projectId, employee.CompanyId);
            if (project?.ProjectBudget == null)
            {
                return new ProjectBudgetStatistics
                {
                    ProjectId = projectId,
                    HasBudget = false,
                    ErrorMessage = "Kein Budget für dieses Projekt definiert."
                };
            }

            // Berechne verwendetes Budget aus allen Tasks
            var usedBudget = CalculateUsedBudget(project.ProjectSections);
            var remainingBudget = project.ProjectBudget.InitialBudget - usedBudget;
            var utilizationPercentage = project.ProjectBudget.InitialBudget > 0
                ? (usedBudget / project.ProjectBudget.InitialBudget) * 100
                : 0;

            // Budget-Status bestimmen
            var budgetStatus = DetermineBudgetStatus(utilizationPercentage);

            // Detaillierte Aufschlüsselung erstellen
            var budgetBreakdown = CreateBudgetBreakdown(project, usedBudget, remainingBudget, utilizationPercentage, budgetStatus);

            // Task-Statistiken berechnen
            var taskStatistics = CalculateTaskBudgetStatistics(project);

            return new ProjectBudgetStatistics
            {
                ProjectId = projectId,
                ProjectName = project.ProjectName,
                HasBudget = true,
                InitialBudget = project.ProjectBudget.InitialBudget,
                UsedBudget = usedBudget,
                RemainingBudget = remainingBudget,
                UtilizationPercentage = utilizationPercentage,
                BudgetStatus = budgetStatus,
                TasksWithCosts = taskStatistics.TasksWithBudget,
                TasksWithoutCosts = taskStatistics.TasksWithoutBudget,
                TotalTasks = taskStatistics.TotalTasks,
                SectionCount = budgetBreakdown.SectionBreakdowns.Count,
                DetailedBreakdown = budgetBreakdown,
                TopCostSections = GetTopCostSections(budgetBreakdown, 5),
                CostDistribution = CalculateCostDistribution(budgetBreakdown)
            };
        }

        /// <summary>
        /// Berechnet erweiterte Projekt-Statistiken inklusive Budget
        /// </summary>
        public async Task<ExtendedProjectStatistics> CalculateExtendedStatisticsAsync(string projectId, ClaimsPrincipal user)
        {
            var basicStats = await CalculateStatisticsAsync(projectId, user);
            var budgetStats = await CalculateBudgetStatisticsAsync(projectId, user);
            var employee = await GetEmployeeAsync(user);

            // Zusätzliche Zeitstatistiken
            var timeStats = await CalculateTimeStatisticsAsync(projectId, employee.CompanyId);

            return new ExtendedProjectStatistics
            {
                BasicStatistics = basicStats,
                BudgetStatistics = budgetStats,
                TimeStatistics = timeStats,
                CalculatedAt = DateTime.UtcNow
            };
        }

        #region Private Budget Calculation Methods

        /// <summary>
        /// Berechnet das verwendete Budget aus allen Project Tasks rekursiv
        /// </summary>
        private double CalculateUsedBudget(ICollection<ProjectSection>? sections)
        {
            if (sections == null) return 0.0;

            double totalUsedBudget = 0.0;

            foreach (var section in sections.Where(ps => ps.ParentSectionId == null))
            {
                totalUsedBudget += CalculateSectionCosts(section);
            }

            return totalUsedBudget;
        }

        /// <summary>
        /// Berechnet Kosten für eine Section und alle ihre Sub-Sections rekursiv
        /// </summary>
        private double CalculateSectionCosts(ProjectSection section)
        {
            double sectionCosts = 0;

            // Kosten aus direkten Tasks
            if (section.ProjectTasks != null)
            {
                foreach (var task in section.ProjectTasks)
                {
                    if (task.TotalCosts.HasValue)
                    {
                        sectionCosts += task.TotalCosts.Value;
                    }
                }
            }

            // Kosten aus Sub-Sections rekursiv
            if (section.SubSections != null)
            {
                foreach (var subSection in section.SubSections)
                {
                    sectionCosts += CalculateSectionCosts(subSection);
                }
            }

            return sectionCosts;
        }

        /// <summary>
        /// Bestimmt den Budget-Status basierend auf der Nutzung
        /// </summary>
        private BudgetStatus DetermineBudgetStatus(double utilizationPercentage)
        {
            return utilizationPercentage switch
            {
                <= 75 => BudgetStatus.OnTrack,
                <= 90 => BudgetStatus.Warning,
                <= 100 => BudgetStatus.Critical,
                > 100 => BudgetStatus.Exceeded,
                _ => BudgetStatus.Unknown
            };
        }

        /// <summary>
        /// Erstellt die detaillierte Budget-Aufschlüsselung
        /// </summary>
        private BudgetBreakdown CreateBudgetBreakdown(Project project, double usedBudget, double remainingBudget, double utilizationPercentage, BudgetStatus status)
        {
            var sectionBreakdowns = new List<SectionBudgetBreakdown>();

            if (project.ProjectSections != null)
            {
                foreach (var section in project.ProjectSections.Where(ps => ps.ParentSectionId == null))
                {
                    var sectionBreakdown = CreateSectionBudgetBreakdown(section, 0);
                    sectionBreakdowns.Add(sectionBreakdown);
                }
            }

            return new BudgetBreakdown
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                InitialBudget = project.ProjectBudget!.InitialBudget,
                TotalUsedBudget = usedBudget,
                RemainingBudget = remainingBudget,
                UtilizationPercentage = utilizationPercentage,
                Status = status,
                SectionBreakdowns = sectionBreakdowns
            };
        }

        /// <summary>
        /// Erstellt detaillierte Aufschlüsselung für eine Section
        /// </summary>
        private SectionBudgetBreakdown CreateSectionBudgetBreakdown(ProjectSection section, int level)
        {
            var taskBreakdowns = new List<TaskBudgetBreakdown>();
            var subSectionBreakdowns = new List<SectionBudgetBreakdown>();
            double sectionTotalCosts = 0.0;

            // Tasks in dieser Section verarbeiten
            if (section.ProjectTasks != null)
            {
                foreach (var task in section.ProjectTasks)
                {
                    var taskCosts = task.TotalCosts ?? 0.0;
                    sectionTotalCosts += taskCosts;

                    taskBreakdowns.Add(new TaskBudgetBreakdown
                    {
                        TaskId = task.ProjectTaskId,
                        TaskName = task.ProjectTaskName ?? "Unnamed Task",
                        StartDate = task.StartDate,
                        EndDate = task.EndDate,
                        DurationInDays = task.DurationInDays,
                        DurationInHours = task.DurationInHours,
                        TotalCosts = taskCosts,
                        WorkerCount = task.ProjectTaskHourlyRateGroups?.Sum(hrg => hrg.Amount) ?? 0,
                        AverageHourlyCost = task.AverageHourlyCost,
                        HasCompleteData = task.IsCalculationDataComplete,
                        HourlyRateGroups = task.ProjectTaskHourlyRateGroups?.Select(hrg => new HRGBreakdown
                        {
                            GroupName = hrg.HourlyRateGroup?.HourlyRateGroupName ?? "Unknown",
                            HourlyRate = (decimal)(hrg.HourlyRateGroup?.HourlyRate ?? 0),
                            WorkerCount = hrg.Amount,
                            GroupCosts = (decimal)(hrg.Amount * (hrg.HourlyRateGroup?.HourlyRate ?? 0))
                        }).ToList() ?? new List<HRGBreakdown>()
                    });
                }
            }

            // Sub-Sections rekursiv verarbeiten
            if (section.SubSections != null)
            {
                foreach (var subSection in section.SubSections)
                {
                    var subSectionBreakdown = CreateSectionBudgetBreakdown(subSection, level + 1);
                    subSectionBreakdowns.Add(subSectionBreakdown);
                    sectionTotalCosts += subSectionBreakdown.TotalCosts;
                }
            }

            return new SectionBudgetBreakdown
            {
                SectionId = section.ProjectSectionId,
                SectionName = section.ProjectSectionName,
                Level = level,
                TotalCosts = sectionTotalCosts,
                TaskCount = taskBreakdowns.Count,
                SubSectionCount = subSectionBreakdowns.Count,
                TaskBreakdowns = taskBreakdowns,
                SubSectionBreakdowns = subSectionBreakdowns
            };
        }

        #endregion

        #region Private Helpers

        private async Task<Employee> GetEmployeeAsync(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            return await new CustomUserManager(_context, _userManager).GetEmployeeAsync(userId);
        }

        private async Task<List<string>> GetProjectSectionIdsAsync(string projectId, string companyId)
        {
            return await _context.ProjectSection
                .Where(ps => ps.ProjectId == projectId && ps.CompanyId == companyId)
                .Select(ps => ps.ProjectSectionId)
                .ToListAsync();
        }

        private async Task<string> GetCompletedStateIdAsync()
        {
            var state = await _context.State
                .FirstOrDefaultAsync(s => s.StateName == "Abgeschlossen")
                ?? throw new InvalidOperationException("Kein Status 'Abgeschlossen' gefunden.");
            return state.StateId;
        }

        private async Task<int> GetTaskCountAsync(
            List<string> sectionIds,
            string companyId,
            string? stateId = null)
        {
            var query = _context.ProjectTask
                .Where(pt => sectionIds.Contains(pt.ProjectSectionId))
                .Where(pt => pt.CompanyId == companyId)
                .Where(pt => pt.IsTaskCatalogEntry);

            if (!string.IsNullOrEmpty(stateId))
            {
                query = query.Where(pt => pt.StateId == stateId);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// Lädt das vollständige Projekt mit allen Budget-relevanten Daten
        /// </summary>
        private async Task<Project?> GetFullProjectAsync(string projectId, string companyId)
        {
            return await _context.Project
                .Include(p => p.ProjectBudget)
                .Include(p => p.ProjectSections.Where(ps => ps.CompanyId == companyId))
                    .ThenInclude(ps => ps.ProjectTasks)
                        .ThenInclude(pt => pt.ProjectTaskHourlyRateGroups)
                            .ThenInclude(hrg => hrg.HourlyRateGroup)
                .Include(p => p.ProjectSections)
                    .ThenInclude(ps => ps.SubSections)
                        .ThenInclude(ss => ss.ProjectTasks)
                            .ThenInclude(pt => pt.ProjectTaskHourlyRateGroups)
                                .ThenInclude(hrg => hrg.HourlyRateGroup)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.CompanyId == companyId);
        }

        /// <summary>
        /// Berechnet Task-spezifische Budget-Statistiken
        /// </summary>
        private TaskBudgetStatistics CalculateTaskBudgetStatistics(Project project)
        {
            var allTasks = GetAllProjectTasks(project.ProjectSections ?? new List<ProjectSection>());

            var tasksWithValidBudget = allTasks
                .Where(t => t.IsCalculationDataComplete && t.TotalCosts.HasValue && t.TotalCosts > 0)
                .ToList();

            return new TaskBudgetStatistics
            {
                TotalTasks = allTasks.Count,
                TasksWithBudget = tasksWithValidBudget.Count,
                TasksWithoutBudget = allTasks.Count - tasksWithValidBudget.Count,
                AverageCostPerTask = tasksWithValidBudget.Any()
                    ? tasksWithValidBudget.Average(t => t.TotalCosts!.Value)
                    : 0,
                HighestTaskCost = tasksWithValidBudget.Any()
                    ? tasksWithValidBudget.Max(t => t.TotalCosts!.Value)
                    : 0,
                LowestTaskCost = tasksWithValidBudget.Any()
                    ? tasksWithValidBudget.Min(t => t.TotalCosts!.Value)
                    : 0,
                TotalWorkingDays = tasksWithValidBudget
                    .Where(t => t.DurationInDays.HasValue)
                    .Sum(t => t.DurationInDays!.Value),
                TotalWorkingHours = tasksWithValidBudget
                    .Where(t => t.DurationInHours.HasValue)
                    .Sum(t => t.DurationInHours!.Value)
            };
        }

        /// <summary>
        /// Berechnet Zeit-basierte Statistiken
        /// </summary>
        private async Task<ProjectTimeStatistics> CalculateTimeStatisticsAsync(string projectId, string companyId)
        {
            var tasks = await _context.ProjectTask
                .Where(pt => pt.ProjectSection!.ProjectId == projectId && pt.CompanyId == companyId)
                .Where(pt => pt.StartDate.HasValue && pt.EndDate.HasValue)
                .Select(pt => new { pt.StartDate, pt.EndDate })
                .ToListAsync();

            if (!tasks.Any())
            {
                return new ProjectTimeStatistics();
            }

            var earliestStart = tasks.Min(t => t.StartDate!.Value);
            var latestEnd = tasks.Max(t => t.EndDate!.Value);
            var totalCalendarDays = (latestEnd.ToDateTime(TimeOnly.MinValue) - earliestStart.ToDateTime(TimeOnly.MinValue)).Days + 1;

            return new ProjectTimeStatistics
            {
                EarliestTaskStart = earliestStart,
                LatestTaskEnd = latestEnd,
                TotalProjectDuration = totalCalendarDays,
                TasksWithDates = tasks.Count
            };
        }

        /// <summary>
        /// Holt alle Tasks aus allen Sections rekursiv
        /// </summary>
        private List<ProjectTask> GetAllProjectTasks(ICollection<ProjectSection> sections)
        {
            var allTasks = new List<ProjectTask>();

            foreach (var section in sections)
            {
                if (section.ProjectTasks != null)
                {
                    allTasks.AddRange(section.ProjectTasks);
                }

                if (section.SubSections != null)
                {
                    allTasks.AddRange(GetAllProjectTasks(section.SubSections));
                }
            }

            return allTasks;
        }

        /// <summary>
        /// Ermittelt die kostenintensivsten Sections
        /// </summary>
        private List<TopCostSection> GetTopCostSections(BudgetBreakdown breakdown, int topCount)
        {
            return breakdown.SectionBreakdowns
                .OrderByDescending(sb => sb.TotalCosts)
                .Take(topCount)
                .Select(sb => new TopCostSection
                {
                    SectionName = sb.SectionName,
                    TotalCosts = sb.TotalCosts,
                    TaskCount = sb.TaskCount,
                    PercentageOfTotal = breakdown.TotalUsedBudget > 0
                        ? (double)(sb.TotalCosts / breakdown.TotalUsedBudget) * 100
                        : 0
                })
                .ToList();
        }

        /// <summary>
        /// Berechnet die Kostenverteilung
        /// </summary>
        private CostDistribution CalculateCostDistribution(BudgetBreakdown breakdown)
        {
            var taskCosts = GetAllTaskCostsFromBreakdown(breakdown.SectionBreakdowns);

            if (!taskCosts.Any())
            {
                return new CostDistribution();
            }

            taskCosts.Sort();
            var totalCosts = taskCosts.Sum();
            var taskCount = taskCosts.Count;

            return new CostDistribution
            {
                MinCost = taskCosts.Min(),
                MaxCost = taskCosts.Max(),
                AverageCost = totalCosts / taskCount,
                MedianCost = taskCount % 2 == 0
                    ? (taskCosts[taskCount / 2 - 1] + taskCosts[taskCount / 2]) / 2
                    : taskCosts[taskCount / 2],
                StandardDeviation = CalculateStandardDeviation(taskCosts, totalCosts / taskCount)
            };
        }

        /// <summary>
        /// Extrahiert alle Task-Kosten aus dem Breakdown
        /// </summary>
        private List<double> GetAllTaskCostsFromBreakdown(List<SectionBudgetBreakdown> sections)
        {
            var costs = new List<double>();

            foreach (var section in sections)
            {
                costs.AddRange(section.TaskBreakdowns.Select(tb => tb.TotalCosts));
                costs.AddRange(GetAllTaskCostsFromBreakdown(section.SubSectionBreakdowns));
            }

            return costs.Where(c => c > 0).ToList();
        }

        /// <summary>
        /// Berechnet die Standardabweichung
        /// </summary>
        private double CalculateStandardDeviation(List<double> values, double mean)
        {
            if (values.Count <= 1) return 0;

            var sumOfSquares = values.Sum(v => (v - mean) * (v - mean));
            var variance = sumOfSquares / (values.Count - 1);
            return Math.Sqrt(variance);
        }

        #endregion
    }

    /// <summary>
    /// Dient als Rückgabemodell für berechnete Projektstatistiken.
    /// </summary>
    public record ProjectStatistics(int TotalTasks, int CompletedTasks)
    {
        public double Progress => TotalTasks == 0 ? 0 : (double)CompletedTasks / TotalTasks;
    }

    /// <summary>
    /// Erweiterte Budget-Statistiken für ein Projekt
    /// </summary>
    public class ProjectBudgetStatistics
    {
        public string ProjectId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public bool HasBudget { get; set; }
        public string? ErrorMessage { get; set; }

        // Budget-Übersicht
        public double InitialBudget { get; set; }
        public double UsedBudget { get; set; }
        public double RemainingBudget { get; set; }
        public double UtilizationPercentage { get; set; }
        public BudgetStatus BudgetStatus { get; set; }

        // Task-Statistiken
        public int TasksWithCosts { get; set; }
        public int TasksWithoutCosts { get; set; }
        public int TotalTasks { get; set; }
        public int SectionCount { get; set; }

        // Detaillierte Aufschlüsselung
        public BudgetBreakdown? DetailedBreakdown { get; set; }
        public List<TopCostSection> TopCostSections { get; set; } = new();
        public CostDistribution CostDistribution { get; set; } = new();
    }

    /// <summary>
    /// Kombinierte erweiterte Projekt-Statistiken
    /// </summary>
    public class ExtendedProjectStatistics
    {
        public ProjectStatistics BasicStatistics { get; set; } = new(0, 0);
        public ProjectBudgetStatistics BudgetStatistics { get; set; } = new();
        public ProjectTimeStatistics TimeStatistics { get; set; } = new();
        public DateTime CalculatedAt { get; set; }
    }

    /// <summary>
    /// Task-spezifische Budget-Statistiken
    /// </summary>
    public class TaskBudgetStatistics
    {
        public int TotalTasks { get; set; }
        public int TasksWithBudget { get; set; }
        public int TasksWithoutBudget { get; set; }
        public double AverageCostPerTask { get; set; }
        public double HighestTaskCost { get; set; }
        public double LowestTaskCost { get; set; }
        public double TotalWorkingDays { get; set; }
        public double TotalWorkingHours { get; set; }
    }

    /// <summary>
    /// Zeit-basierte Projekt-Statistiken
    /// </summary>
    public class ProjectTimeStatistics
    {
        public DateOnly? EarliestTaskStart { get; set; }
        public DateOnly? LatestTaskEnd { get; set; }
        public int TotalProjectDuration { get; set; }
        public int TasksWithDates { get; set; }
    }

    /// <summary>
    /// Top-Kosten-Section für Ranking
    /// </summary>
    public class TopCostSection
    {
        public string SectionName { get; set; } = string.Empty;
        public double TotalCosts { get; set; }
        public int TaskCount { get; set; }
        public double PercentageOfTotal { get; set; }
    }

    /// <summary>
    /// Kostenverteilungs-Statistiken
    /// </summary>
    public class CostDistribution
    {
        public double MinCost { get; set; }
        public double MaxCost { get; set; }
        public double AverageCost { get; set; }
        public double MedianCost { get; set; }
        public double StandardDeviation { get; set; }
    }

    // ---------- Supporting Classes for Budget Analysis ----------

    public enum BudgetStatus
    {
        Unknown,
        OnTrack,     // <= 75%
        Warning,     // 76-90%
        Critical,    // 91-100%
        Exceeded     // > 100%
    }

    public class BudgetBreakdown
    {
        public string ProjectId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public double InitialBudget { get; set; }
        public double TotalUsedBudget { get; set; }
        public double RemainingBudget { get; set; }
        public double UtilizationPercentage { get; set; }
        public BudgetStatus Status { get; set; }
        public List<SectionBudgetBreakdown> SectionBreakdowns { get; set; } = new();
    }

    public class SectionBudgetBreakdown
    {
        public string SectionId { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int Level { get; set; }
        public double TotalCosts { get; set; }
        public int TaskCount { get; set; }
        public int SubSectionCount { get; set; }
        public List<TaskBudgetBreakdown> TaskBreakdowns { get; set; } = new();
        public List<SectionBudgetBreakdown> SubSectionBreakdowns { get; set; } = new();
    }

    public class TaskBudgetBreakdown
    {
        public string TaskId { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public double? DurationInDays { get; set; }
        public double? DurationInHours { get; set; }
        public double TotalCosts { get; set; }
        public int WorkerCount { get; set; }
        public decimal? AverageHourlyCost { get; set; }
        public bool HasCompleteData { get; set; }
        public List<HRGBreakdown> HourlyRateGroups { get; set; } = new();
    }

    public class HRGBreakdown
    {
        public string GroupName { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public int WorkerCount { get; set; }
        public decimal GroupCosts { get; set; }
    }
}