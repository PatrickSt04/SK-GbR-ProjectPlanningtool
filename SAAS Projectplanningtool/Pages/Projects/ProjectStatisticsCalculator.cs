using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.Models.TimeTracking;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class ProjectStatisticsCalculator(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly UserManager<IdentityUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

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
        /// Berechnet detaillierte Budget-Statistiken für ein Projekt.
        /// Kosten basieren auf Zeitbuchungen: NetWorkingHours * Employee.HourlyRateGroup.HourlyRate
        /// </summary>
        public async Task<ProjectBudgetStatistics> CalculateBudgetStatisticsAsync(string projectId, ClaimsPrincipal user)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                throw new ArgumentException("Projekt-ID darf nicht leer sein.", nameof(projectId));

            var employee = await GetEmployeeAsync(user);

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

            // Kosten aus Zeitbuchungen berechnen
            var usedBudgetFromTimeEntries = await CalculateUsedBudgetFromTimeEntriesAsync(projectId);

            // Zusätzliche Projektkosten
            var additionalCosts = await CalculateAdditionalProjectCostsAsync(projectId);

            // Gesamtverbrauch = Zeitbuchungen + Zusätzliche Kosten
            var totalUsedBudget = usedBudgetFromTimeEntries + additionalCosts.TotalAmount;

            var remainingBudget = 0.0;
            var utilizationPercentage = 0.0;

            if (project.ProjectBudget.BudgetRecalculations?.Count > 0)
            {
                var latestRecalculation = project.ProjectBudget.BudgetRecalculations
                    .OrderByDescending(br => br.RecalculationDateTime)
                    .FirstOrDefault();

                if (latestRecalculation != null)
                {
                    remainingBudget = latestRecalculation.NewBudget - totalUsedBudget;
                    utilizationPercentage = latestRecalculation.NewBudget > 0
                        ? (totalUsedBudget / latestRecalculation.NewBudget) * 100
                        : 0;
                }
            }
            else
            {
                remainingBudget = project.ProjectBudget.InitialBudget - totalUsedBudget;
                utilizationPercentage = project.ProjectBudget.InitialBudget > 0
                    ? (totalUsedBudget / project.ProjectBudget.InitialBudget) * 100
                    : 0;
            }

            var budgetStatus = DetermineBudgetStatus(utilizationPercentage);
            var budgetBreakdown = CreateBudgetBreakdown(project, usedBudgetFromTimeEntries, remainingBudget, utilizationPercentage, budgetStatus, additionalCosts);
            var taskStatistics = CalculateTaskBudgetStatistics(project);

            var timeTrackingBreakdown = await CalculateTimeTrackingBreakdownAsync(projectId);
            return new ProjectBudgetStatistics
            {
                ProjectId = projectId,
                ProjectName = project.ProjectName,
                HasBudget = true,
                recalculationNeeded = budgetStatus == BudgetStatus.Exceeded,
                InitialBudget = project.ProjectBudget.InitialBudget,
                UsedBudget = totalUsedBudget,
                UsedBudgetFromTimeEntries = usedBudgetFromTimeEntries,
                AdditionalCosts = additionalCosts.TotalAmount,
                RemainingBudget = remainingBudget,
                UtilizationPercentage = utilizationPercentage,
                BudgetStatus = budgetStatus,
                TasksWithDates = taskStatistics.TasksWithDates,
                TasksWithoutDates = taskStatistics.TasksWithoutDates,
                TotalTasks = taskStatistics.TotalTasks,
                SectionCount = budgetBreakdown.SectionBreakdowns.Count,
                DetailedBreakdown = budgetBreakdown,
                TopCostSections = GetTopCostSections(budgetBreakdown, 5),
                CostDistribution = CalculateCostDistribution(budgetBreakdown),
                AdditionalCostBreakdown = additionalCosts,
                TimeTrackingBreakdown = timeTrackingBreakdown
            };
        }

        public async Task<List<TimeTrackingBreakdownItem>> CalculateTimeTrackingBreakdownAsync(string projectId)
        {
            var timeEntries = await _context.TimeEntry
                .Where(te => te.ProjectId == projectId)
                .Include(te => te.Employee)
                    .ThenInclude(e => e.HourlyRateGroup)
                .ToListAsync();

            var breakdown = timeEntries
                .GroupBy(te => te.Employee?.HourlyRateGroup?.HourlyRateGroupName ?? "Ohne Stundensatzgruppe")
                .Select(group =>
                {
                    var totalHours = group.Sum(te => te.NetWorkingHours);
                    var hourlyRate = group.First().Employee?.HourlyRateGroup?.HourlyRate ?? 0;

                    return new TimeTrackingBreakdownItem
                    {
                        HourlyRateGroupName = group.Key,
                        TotalWorkingHours = totalHours,
                        TotalCosts = totalHours * hourlyRate
                    };
                })
                .OrderByDescending(b => b.TotalCosts)
                .ToList();

            return breakdown;
        }

        /// <summary>
        /// Berechnet erweiterte Projekt-Statistiken inklusive Budget
        /// </summary>
        public async Task<ExtendedProjectStatistics> CalculateExtendedStatisticsAsync(string projectId, ClaimsPrincipal user)
        {
            var basicStats = await CalculateStatisticsAsync(projectId, user);
            var budgetStats = await CalculateBudgetStatisticsAsync(projectId, user);
            var employee = await GetEmployeeAsync(user);
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
        /// Berechnet das verwendete Budget aus allen Zeitbuchungen des Projekts.
        /// Formel: NetWorkingHours * Employee.HourlyRateGroup.HourlyRate
        /// </summary>
        private async Task<double> CalculateUsedBudgetFromTimeEntriesAsync(string projectId)
        {
            var timeEntries = await _context.TimeEntry
                .Where(te => te.ProjectId == projectId)
                .Include(te => te.Employee)
                    .ThenInclude(e => e.HourlyRateGroup)
                .ToListAsync();

            double totalCosts = 0;

            foreach (var entry in timeEntries)
            {
                var hourlyRate = entry.Employee?.HourlyRateGroup?.HourlyRate;
                if (hourlyRate == null)
                    continue;

                totalCosts += entry.NetWorkingHours * (double)hourlyRate;
            }

            return totalCosts;
        }

        /// <summary>
        /// Berechnet die zusätzlichen Projektkosten
        /// </summary>
        private async Task<AdditionalCostBreakdown> CalculateAdditionalProjectCostsAsync(string projectId)
        {
            var additionalCosts = await _context.ProjectAdditionalCosts
                .Where(c => c.Project.ProjectId == projectId)
                .Select(c => new AdditionalCostItem
                {
                    CostId = c.ProjectAdditionalCostsId,
                    CostName = c.AdditionalCostName ?? "Unbenannt",
                    Amount = c.AdditionalCostAmount ?? 0,
                    CreatedTimestamp = c.CreatedTimestamp
                })
                .ToListAsync();

            return new AdditionalCostBreakdown
            {
                TotalAmount = additionalCosts.Sum(c => c.Amount),
                ItemCount = additionalCosts.Count,
                Items = additionalCosts
            };
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
        /// Erstellt die detaillierte Budget-Aufschlüsselung auf Section-Ebene
        /// </summary>
        private BudgetBreakdown CreateBudgetBreakdown(Project project, double usedBudgetFromTimeEntries,
            double remainingBudget, double utilizationPercentage, BudgetStatus status,
            AdditionalCostBreakdown additionalCosts)
        {
            var sectionBreakdowns = new List<SectionBudgetBreakdown>();

            if (project.ProjectSections != null)
            {
                foreach (var section in project.ProjectSections.Where(ps => ps.ParentSectionId == null))
                {
                    sectionBreakdowns.Add(CreateSectionBudgetBreakdown(section, 0));
                }
            }

            return new BudgetBreakdown
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                InitialBudget = project.ProjectBudget!.InitialBudget,
                TotalUsedBudget = usedBudgetFromTimeEntries + additionalCosts.TotalAmount,
                UsedBudgetFromTimeEntries = usedBudgetFromTimeEntries,
                AdditionalCosts = additionalCosts.TotalAmount,
                RemainingBudget = remainingBudget,
                UtilizationPercentage = utilizationPercentage,
                Status = status,
                SectionBreakdowns = sectionBreakdowns,
                AdditionalCostBreakdown = additionalCosts
            };
        }

        /// <summary>
        /// Erstellt die Aufschlüsselung einer Section (strukturell, ohne Einzelkosten pro Task)
        /// </summary>
        private SectionBudgetBreakdown CreateSectionBudgetBreakdown(ProjectSection section, int level)
        {
            var taskBreakdowns = new List<TaskBudgetBreakdown>();
            var subSectionBreakdowns = new List<SectionBudgetBreakdown>();

            if (section.ProjectTasks != null)
            {
                foreach (var task in section.ProjectTasks)
                {
                    taskBreakdowns.Add(new TaskBudgetBreakdown
                    {
                        TaskId = task.ProjectTaskId,
                        TaskName = task.ProjectTaskName ?? "Unnamed Task",
                        StartDate = task.StartDate,
                        EndDate = task.EndDate,
                        DurationInDays = task.DurationInDays,
                        DurationInHours = task.DurationInHours
                    });
                }
            }

            if (section.SubSections != null)
            {
                foreach (var subSection in section.SubSections)
                {
                    subSectionBreakdowns.Add(CreateSectionBudgetBreakdown(subSection, level + 1));
                }
            }

            return new SectionBudgetBreakdown
            {
                SectionId = section.ProjectSectionId,
                SectionName = section.ProjectSectionName,
                Level = level,
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

        private async Task<int> GetTaskCountAsync(List<string> sectionIds, string companyId, string? stateId = null)
        {
            var query = _context.ProjectTask
                .Where(pt => sectionIds.Contains(pt.ProjectSectionId))
                .Where(pt => pt.CompanyId == companyId);

            if (!string.IsNullOrEmpty(stateId))
                query = query.Where(pt => pt.StateId == stateId);

            return await query.CountAsync();
        }

        /// <summary>
        /// Lädt das vollständige Projekt mit allen Budget-relevanten Daten.
        /// TimeEntries werden separat geladen, da sie projektbezogen sind.
        /// </summary>
        private async Task<Project?> GetFullProjectAsync(string projectId, string companyId)
        {
            return await _context.Project
                .Include(p => p.ProjectBudget)
                    .ThenInclude(pb => pb.BudgetRecalculations)
                .Include(p => p.ProjectAdditionalCosts)
                .Include(p => p.ProjectSections.Where(ps => ps.CompanyId == companyId))
                    .ThenInclude(ps => ps.ProjectTasks)
                .Include(p => p.ProjectSections)
                    .ThenInclude(ps => ps.SubSections)
                        .ThenInclude(ss => ss.ProjectTasks)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.CompanyId == companyId);
        }

        /// <summary>
        /// Berechnet Task-spezifische Statistiken (nur noch Datumsbasiert, keine Kosten mehr auf Taskebene)
        /// </summary>
        private TaskBudgetStatistics CalculateTaskBudgetStatistics(Project project)
        {
            var allTasks = GetAllProjectTasks(project.ProjectSections ?? new List<ProjectSection>());

            var tasksWithDates = allTasks
                .Where(t => t.StartDate.HasValue && t.EndDate.HasValue)
                .ToList();

            return new TaskBudgetStatistics
            {
                TotalTasks = allTasks.Count,
                TasksWithDates = tasksWithDates.Count,
                TasksWithoutDates = allTasks.Count - tasksWithDates.Count,
                TotalWorkingDays = tasksWithDates
                    .Where(t => t.DurationInDays.HasValue)
                    .Sum(t => t.DurationInDays!.Value),
                TotalWorkingHours = tasksWithDates
                    .Where(t => t.DurationInHours.HasValue)
                    .Sum(t => t.DurationInHours!.Value)
            };
        }

        private async Task<ProjectTimeStatistics> CalculateTimeStatisticsAsync(string projectId, string companyId)
        {
            var tasks = await _context.ProjectTask
                .Where(pt => pt.ProjectSection!.ProjectId == projectId && pt.CompanyId == companyId)
                .Where(pt => pt.StartDate.HasValue && pt.EndDate.HasValue)
                .Select(pt => new { pt.StartDate, pt.EndDate })
                .ToListAsync();

            if (!tasks.Any())
                return new ProjectTimeStatistics();

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

        private List<ProjectTask> GetAllProjectTasks(ICollection<ProjectSection> sections)
        {
            var allTasks = new List<ProjectTask>();

            foreach (var section in sections)
            {
                if (section.ProjectTasks != null)
                    allTasks.AddRange(section.ProjectTasks);

                if (section.SubSections != null)
                    allTasks.AddRange(GetAllProjectTasks(section.SubSections));
            }

            return allTasks;
        }

        private List<TopCostSection> GetTopCostSections(BudgetBreakdown breakdown, int topCount)
        {
            return breakdown.SectionBreakdowns
                .OrderByDescending(sb => sb.TaskCount)
                .Take(topCount)
                .Select(sb => new TopCostSection
                {
                    SectionName = sb.SectionName,
                    TaskCount = sb.TaskCount
                })
                .ToList();
        }

        private CostDistribution CalculateCostDistribution(BudgetBreakdown breakdown)
        {
            // Kostenverteilung auf Task-Ebene entfällt, da Kosten nur noch projektbezogen erfasst werden
            return new CostDistribution();
        }

        #endregion
    }

    public record ProjectStatistics(int TotalTasks, int CompletedTasks)
    {
        public double Progress => TotalTasks == 0 ? 0 : (double)CompletedTasks / TotalTasks;
    }

    public class ProjectBudgetStatistics
    {
        public string ProjectId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public bool HasBudget { get; set; }
        public bool recalculationNeeded { get; set; }
        public string? ErrorMessage { get; set; }

        public double InitialBudget { get; set; }
        public double UsedBudget { get; set; }

        /// <summary>Kosten aus Zeitbuchungen (NetWorkingHours * HourlyRate)</summary>
        public double UsedBudgetFromTimeEntries { get; set; }
        public double AdditionalCosts { get; set; }
        public double RemainingBudget { get; set; }
        public double UtilizationPercentage { get; set; }
        public BudgetStatus BudgetStatus { get; set; }

        public int TasksWithDates { get; set; }
        public int TasksWithoutDates { get; set; }
        public int TotalTasks { get; set; }
        public int SectionCount { get; set; }

        public BudgetBreakdown? DetailedBreakdown { get; set; }
        public List<TopCostSection> TopCostSections { get; set; } = new();
        public CostDistribution CostDistribution { get; set; } = new();
        public AdditionalCostBreakdown AdditionalCostBreakdown { get; set; } = new();

        public List<TimeTrackingBreakdownItem> TimeTrackingBreakdown { get; set; } = new();
    }
    public class TimeTrackingBreakdownItem
    {
        public string HourlyRateGroupName { get; set; } = string.Empty;
        public double TotalWorkingHours { get; set; }
        public double TotalCosts { get; set; }
    }
    public class AdditionalCostBreakdown
    {
        public double TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public List<AdditionalCostItem> Items { get; set; } = new();
    }

    public class AdditionalCostItem
    {
        public string CostId { get; set; } = string.Empty;
        public string CostName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
    }

    public class ExtendedProjectStatistics
    {
        public ProjectStatistics BasicStatistics { get; set; } = new(0, 0);
        public ProjectBudgetStatistics BudgetStatistics { get; set; } = new();
        public ProjectTimeStatistics TimeStatistics { get; set; } = new();
        public DateTime CalculatedAt { get; set; }
    }

    public class TaskBudgetStatistics
    {
        public int TotalTasks { get; set; }
        public int TasksWithDates { get; set; }
        public int TasksWithoutDates { get; set; }
        public double TotalWorkingDays { get; set; }
        public double TotalWorkingHours { get; set; }
    }

    public class ProjectTimeStatistics
    {
        public DateOnly? EarliestTaskStart { get; set; }
        public DateOnly? LatestTaskEnd { get; set; }
        public int TotalProjectDuration { get; set; }
        public int TasksWithDates { get; set; }
    }

    public class TopCostSection
    {
        public string SectionName { get; set; } = string.Empty;
        public int TaskCount { get; set; }
    }

    public class CostDistribution
    {
        public double MinCost { get; set; }
        public double MaxCost { get; set; }
        public double AverageCost { get; set; }
        public double MedianCost { get; set; }
        public double StandardDeviation { get; set; }
    }

    public enum BudgetStatus
    {
        Unknown,
        OnTrack,
        Warning,
        Critical,
        Exceeded
    }

    public class BudgetBreakdown
    {
        public string ProjectId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public double InitialBudget { get; set; }
        public double TotalUsedBudget { get; set; }
        public double UsedBudgetFromTimeEntries { get; set; }
        public double AdditionalCosts { get; set; }
        public double RemainingBudget { get; set; }
        public double UtilizationPercentage { get; set; }
        public BudgetStatus Status { get; set; }
        public List<SectionBudgetBreakdown> SectionBreakdowns { get; set; } = new();
        public AdditionalCostBreakdown AdditionalCostBreakdown { get; set; } = new();
    }

    public class SectionBudgetBreakdown
    {
        public string SectionId { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int Level { get; set; }
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
    }
}