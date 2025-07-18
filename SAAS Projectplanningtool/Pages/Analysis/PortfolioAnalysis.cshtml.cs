using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAAS_Projectplanningtool.Pages.Analysis
{
    public class PortfolioAnalysisModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PortfolioAnalysisModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // KPI Properties
        public int ActiveProjects { get; set; }
        public decimal DelayRate { get; set; }
        public decimal CompletionRate { get; set; }
        public int MedianRemainingDays { get; set; }
        public decimal ChangeIntensity { get; set; }

        // Chart Data
        public StatusDistributionData StatusDistribution { get; set; }
        public List<ProjectLifecycleData> LifecycleTrend { get; set; }
        public List<OverdueProject> OverdueProjects { get; set; }
        public List<ProjectProgressData> ProgressVsTime { get; set; }
        public List<ChangeHotspot> ChangeHotspots { get; set; }
        public List<RiskProject> RiskProjects { get; set; }

        // Filter Properties
        [BindProperty(SupportsGet = true)]
        public string TimeFilter { get; set; } = "month";

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = "all";

        public async Task OnGetAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var companyId = GetCurrentCompanyId(); // Implement based on your auth logic

            // Load projects with related data
            var projects = await _context.Project
                .Include(p => p.State)
                .Include(p => p.ResponsiblePerson)
                .Include(p => p.ProjectBudget)
                .Include(p => p.ProjectSections)
                    .ThenInclude(s => s.ProjectTasks)
                        .ThenInclude(t => t.State)
                .Where(p => p.CompanyId == companyId)
                .ToListAsync();

            // Calculate KPIs
            await CalculateKPIs(projects, today);

            // Prepare chart data
            await PrepareChartData(projects, today);
        }

        private async Task CalculateKPIs(List<Project> projects, DateOnly today)
        {
            // 1. Active Projects
            var activeProjects = projects.Where(p => p.IsArchived != true).ToList();
            ActiveProjects = activeProjects.Count;

            // 2. Delay Rate
            var delayedProjects = activeProjects.Where(p =>
                p.EndDate.HasValue &&
                p.EndDate.Value < today &&
                p.State?.StateName != "Abgeschlossen"
            ).Count();
            DelayRate = ActiveProjects > 0 ? (decimal)delayedProjects / ActiveProjects * 100 : 0;

            // 3. Completion Rate
            var allTasks = activeProjects
                .SelectMany(p => p.ProjectSections ?? new List<ProjectSection>())
                .SelectMany(s => s.ProjectTasks ?? new List<ProjectTask>())
                .ToList();

            var completedTasks = allTasks.Count(t => t.State?.StateName == "Abgeschlossen");
            CompletionRate = allTasks.Any() ? (decimal)completedTasks / allTasks.Count * 100 : 0;

            // 4. Median Remaining Days
            var remainingDays = activeProjects
                .Where(p => p.EndDate.HasValue)
                .Select(p => (p.EndDate.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days)
                .Where(d => d > 0)
                .OrderBy(d => d)
                .ToList();

            MedianRemainingDays = remainingDays.Any() ?
                remainingDays[remainingDays.Count / 2] : 0;

            // 5. Change Intensity (last 7 days)
            var sevenDaysAgo = DateTime.Now.AddDays(-7);
            var recentChanges = activeProjects.Count(p =>
                p.LatestModificationTimestamp.HasValue &&
                p.LatestModificationTimestamp.Value >= sevenDaysAgo
            );
            ChangeIntensity = ActiveProjects > 0 ?
                (decimal)recentChanges / ActiveProjects : 0;
        }

        private async Task PrepareChartData(List<Project> projects, DateOnly today)
        {
            // Status Distribution
            var statusGroups = projects
                .Where(p => p.IsArchived != true)
                .GroupBy(p => p.State?.StateName ?? "Undefined")
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            StatusDistribution = new StatusDistributionData
            {
                Labels = statusGroups.Select(s => s.Status).ToList(),
                Values = statusGroups.Select(s => s.Count).ToList(),
                Total = projects.Count(p => p.IsArchived != true)
            };

            // Progress vs Time Scatter
            ProgressVsTime = projects
                .Where(p => p.IsArchived != true && p.StartDate.HasValue && p.EndDate.HasValue)
                .Select(p => {
                    var totalDuration = (p.EndDate.Value.ToDateTime(TimeOnly.MinValue) - p.StartDate.Value.ToDateTime(TimeOnly.MinValue)).Days;
                    var elapsedDuration = (DateTime.Today - p.StartDate.Value.ToDateTime(TimeOnly.MinValue)).Days;
                    var timeProgress = totalDuration > 0 ? (decimal)elapsedDuration / totalDuration * 100 : 0;

                    var tasks = p.ProjectSections?.SelectMany(s => s.ProjectTasks ?? new List<ProjectTask>()) ?? new List<ProjectTask>();
                    var taskProgress = tasks.Any() ?
                        (decimal)tasks.Count(t => t.State?.StateName == "Abgeschlossen") / tasks.Count() * 100 : 0;

                    var budgetUsage = p.ProjectBudget != null && p.ProjectBudget.InitialBudget > 0 ?
                        (decimal)p.ProjectBudget.UsedBudget / (decimal)p.ProjectBudget.InitialBudget * 100 : 0;

                    return new ProjectProgressData
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        TimeProgress = Math.Min(timeProgress, 100),
                        TaskProgress = taskProgress,
                        BudgetUsage = (float)budgetUsage,
                        InitialBudget = p.ProjectBudget?.InitialBudget ?? 0
                    };
                })
                .ToList();

            // Overdue Projects
            OverdueProjects = projects
                .Where(p => p.IsArchived != true &&
                       p.EndDate.HasValue &&
                       p.EndDate.Value < today &&
                       p.State?.StateName != "Abgeschlossen")
                .Select(p => {
                    var daysOverdue = (today.ToDateTime(TimeOnly.MinValue) - p.EndDate.Value.ToDateTime(TimeOnly.MinValue)).Days;
                    var tasks = p.ProjectSections?.SelectMany(s => s.ProjectTasks ?? new List<ProjectTask>()) ?? new List<ProjectTask>();
                    var taskProgress = tasks.Any() ?
                        (decimal)tasks.Count(t => t.State?.StateName == "Abgeschlossen") / tasks.Count() * 100 : 0;

                    return new OverdueProject
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        DaysOverdue = daysOverdue,
                        CompletionRate = taskProgress,
                        ResponsiblePerson = p.ResponsiblePerson?.EmployeeDisplayName ?? "N/A",
                        BudgetRemaining = p.ProjectBudget != null ?
                            p.ProjectBudget.InitialBudget - p.ProjectBudget.UsedBudget : 0
                    };
                })
                .OrderByDescending(p => p.DaysOverdue)
                .ToList();

            // Change Hotspots (last 30 days)
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            ChangeHotspots = projects
                .Where(p => p.IsArchived != true &&
                       p.LatestModificationTimestamp.HasValue &&
                       p.LatestModificationTimestamp.Value >= thirtyDaysAgo)
                .GroupBy(p => p.ProjectId)
                .Select(g => new ChangeHotspot
                {
                    ProjectId = g.Key,
                    ProjectName = g.First().ProjectName,
                    ChangeCount = g.Count(),
                    LastChange = g.Max(p => p.LatestModificationTimestamp.Value)
                })
                .OrderByDescending(c => c.ChangeCount)
                .Take(5)
                .ToList();

            // Risk Projects (composite score)
            RiskProjects = projects
                .Where(p => p.IsArchived != true)
                .Select(p => {
                    var riskScore = 0m;

                    // Overdue factor
                    if (p.EndDate.HasValue && p.EndDate.Value < today && p.State?.StateName != "Abgeschlossen")
                    {
                        var daysOverdue = (today.ToDateTime(TimeOnly.MinValue) - p.EndDate.Value.ToDateTime(TimeOnly.MinValue)).Days;
                        riskScore += Math.Min(daysOverdue / 10m, 5m); // Max 5 points for overdue
                    }

                    // Budget overrun factor
                    if (p.ProjectBudget != null && p.ProjectBudget.InitialBudget > 0)
                    {
                        var budgetUsage = (decimal)p.ProjectBudget.UsedBudget / (decimal)p.ProjectBudget.InitialBudget;
                        if (budgetUsage > 0.9m) riskScore += (budgetUsage - 0.9m) * 10; // Points for >90% usage
                    }

                    // Low completion rate factor
                    var tasks = p.ProjectSections?.SelectMany(s => s.ProjectTasks ?? new List<ProjectTask>()) ?? new List<ProjectTask>();
                    if (tasks.Any())
                    {
                        var completionRate = (decimal)tasks.Count(t => t.State?.StateName == "Abgeschlossen") / tasks.Count();
                        var expectedCompletion = 0.5m; // Simplified - should be based on time progress
                        if (completionRate < expectedCompletion)
                        {
                            riskScore += (expectedCompletion - completionRate) * 5;
                        }
                    }

                    return new RiskProject
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        RiskScore = riskScore,
                        Status = p.State?.StateName ?? "Undefined",
                        ResponsiblePerson = p.ResponsiblePerson?.EmployeeDisplayName ?? "N/A"
                    };
                })
                .OrderByDescending(r => r.RiskScore)
                .Take(5)
                .ToList();

            // Lifecycle Trend (simplified - last 12 weeks)
            var weeklyData = new List<ProjectLifecycleData>();
            for (int i = 11; i >= 0; i--)
            {
                var weekStart = DateTime.Today.AddDays(-7 * i);
                var weekEnd = weekStart.AddDays(7);

                // This is simplified - in production, you'd need historical status data
                weeklyData.Add(new ProjectLifecycleData
                {
                    Week = weekStart.ToString("MMM dd"),
                    Planning = projects.Count(p => p.State?.StateName == "Planung"),
                    InProgress = projects.Count(p => p.State?.StateName == "In Bearbeitung"),
                    Review = projects.Count(p => p.State?.StateName == "Review"),
                    Completed = projects.Count(p => p.State?.StateName == "Abgeschlossen")
                });
            }
            LifecycleTrend = weeklyData;
        }

        private string GetCurrentCompanyId()
        {
            // Implement based on your authentication/authorization logic
            // For now, return a placeholder
            return User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value ?? "";
        }

        // Data Models
        public class StatusDistributionData
        {
            public List<string> Labels { get; set; }
            public List<int> Values { get; set; }
            public int Total { get; set; }
        }

        public class ProjectLifecycleData
        {
            public string Week { get; set; }
            public int Planning { get; set; }
            public int InProgress { get; set; }
            public int Review { get; set; }
            public int Completed { get; set; }
        }

        public class OverdueProject
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public int DaysOverdue { get; set; }
            public decimal CompletionRate { get; set; }
            public string ResponsiblePerson { get; set; }
            public float BudgetRemaining { get; set; }
        }

        public class ProjectProgressData
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public decimal TimeProgress { get; set; }
            public decimal TaskProgress { get; set; }
            public float BudgetUsage { get; set; }
            public float InitialBudget { get; set; }
        }

        public class ChangeHotspot
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public int ChangeCount { get; set; }
            public DateTime LastChange { get; set; }
        }

        public class RiskProject
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public decimal RiskScore { get; set; }
            public string Status { get; set; }
            public string ResponsiblePerson { get; set; }
        }
    }
}