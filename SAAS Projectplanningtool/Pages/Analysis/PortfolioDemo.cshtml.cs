using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SAAS_Projectplanningtool.Pages.Analysis;

public class PortfolioDemoModel : PageModel
{
    // ---------------------- Data Models ----------------------
    public class ProjectTask
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Open"; // Open, InProgress, Done
        public int ProgressPercent { get; set; } // 0..100 (optional)
    }

    public class Project
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsArchived { get; set; }
        public string Status { get; set; } = "Open"; // Open, InProgress, Done
        public List<ProjectTask> Tasks { get; set; } = new();
    }

    // ---------------------- Public View Data ----------------------
    public List<Project> Projects { get; set; } = new();

    // KPI values
    public int ActiveProjects => Projects.Count(p => !p.IsArchived);

    public double DelayRatePercent => ActiveProjects == 0 ? 0 : Math.Round(
        Projects.Count(p => !p.IsArchived && DateTime.Today > p.EndDate && p.Status != "Done") * 100.0 / ActiveProjects, 1);

    public double PortfolioCompletionPercent
    {
        get
        {
            var allTasks = Projects.Where(p => !p.IsArchived).SelectMany(p => p.Tasks).ToList();
            if (!allTasks.Any()) return 0;
            return Math.Round(allTasks.Count(t => t.Status == "Done") * 100.0 / allTasks.Count, 1);
        }
    }

    public int MedianRemainingDays
    {
        get
        {
            var remaining = Projects.Where(p => !p.IsArchived)
                .Select(p => (p.EndDate - DateTime.Today).TotalDays)
                .Where(d => d >= 0).OrderBy(d => d).ToList();
            if (!remaining.Any()) return 0;
            int mid = remaining.Count / 2;
            double median = remaining.Count % 2 == 0 ? (remaining[mid - 1] + remaining[mid]) / 2.0 : remaining[mid];
            return (int)Math.Round(median);
        }
    }

    public double ChangeIntensityProxy { get; set; }

    // Chart / table source data
    public List<(string Status, int Count)> StatusDistribution { get; set; } = new();
    public List<(string Project, double TimeProgress, double Completion)> ProgressScatter { get; set; } = new();
    public List<(string Project, int OverdueDays, double Completion)> OverdueProjects { get; set; } = new();

    // Derived
    public int ProjectsBehindPlan => ProgressScatter.Count(p => p.Completion + 5 < p.TimeProgress);
    public int ProjectsOnOrAhead => ProgressScatter.Count - ProjectsBehindPlan;
    public List<(string Project, double TimeProgress, double Completion, double Gap)> BehindProjects { get; set; } = new();

    // JSON strings for safe embedding
    public string StatusLabelsJson { get; set; } = "[]";
    public string StatusCountsJson { get; set; } = "[]";
    public string ScatterJson { get; set; } = "[]";

    // ---------------------- OnGet: Build Mock Data ----------------------
    public void OnGet()
    {
        var rand = new Random(11);
        var today = DateTime.Today;

        Projects = Enumerable.Range(1, 12).Select(i =>
        {
            int duration = rand.Next(30, 160);
            var start = today.AddDays(-rand.Next(0, 90));
            var end = start.AddDays(duration);
            var status = i % 5 == 0 ? "Done" : (i % 3 == 0 ? "InProgress" : "Open");
            var tasks = Enumerable.Range(1, rand.Next(4, 10)).Select(ti => new ProjectTask
            {
                Name = $"P{i}-T{ti}",
                StartDate = start.AddDays(rand.Next(0, duration / 2)),
                EndDate = start.AddDays(rand.Next(duration / 2, duration)),
                Status = rand.NextDouble() < 0.4 ? "Done" : (rand.NextDouble() < 0.5 ? "InProgress" : "Open"),
                ProgressPercent = rand.Next(0, 101)
            }).ToList();
            if (status == "Done")
            {
                foreach (var t in tasks) { t.Status = "Done"; t.ProgressPercent = 100; }
            }
            return new Project
            {
                Name = $"Projekt {i}",
                StartDate = start,
                EndDate = end,
                Status = status,
                IsArchived = false,
                Tasks = tasks
            };
        }).ToList();

        StatusDistribution = Projects
            .GroupBy(p => p.Status)
            .Select(g => (Status: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();

        ProgressScatter = Projects
            .Where(p => !p.IsArchived)
            .Select(p =>
            {
                double totalDuration = (p.EndDate - p.StartDate).TotalDays;
                double elapsed = (DateTime.Today - p.StartDate).TotalDays;
                var timeProgress = totalDuration <= 0 ? 0 : Math.Clamp(elapsed / totalDuration * 100.0, 0, 150);
                double completion = !p.Tasks.Any() ? 0 : p.Tasks.Count(t => t.Status == "Done") * 100.0 / p.Tasks.Count;
                return (Project: p.Name, TimeProgress: timeProgress, Completion: completion);
            })
            .ToList();

        BehindProjects = ProgressScatter
            .Where(p => p.Completion + 5 < p.TimeProgress)
            .Select(p => (p.Project, p.TimeProgress, p.Completion, Gap: p.Completion - p.TimeProgress))
            .OrderBy(p => p.Gap)
            .Take(8)
            .ToList();

        OverdueProjects = Projects
            .Where(p => DateTime.Today > p.EndDate && p.Status != "Done")
            .Select(p => (
                Project: p.Name,
                OverdueDays: (DateTime.Today - p.EndDate).Days,
                Completion: p.Tasks.Any() ? p.Tasks.Count(t => t.Status == "Done") * 100.0 / p.Tasks.Count : 0
            ))
            .OrderByDescending(x => x.OverdueDays)
            .ToList();

        ChangeIntensityProxy = Math.Round(Projects.Count * 0.6 + rand.NextDouble() * 2.0, 1);

        // Serialize (Invariant formatting already applied via rounding for numeric arrays)
        StatusLabelsJson = JsonSerializer.Serialize(StatusDistribution.Select(s => s.Status));
        StatusCountsJson = JsonSerializer.Serialize(StatusDistribution.Select(s => s.Count));
        ScatterJson = JsonSerializer.Serialize(ProgressScatter.Select(p => new { x = Math.Round(p.TimeProgress, 1), y = Math.Round(p.Completion, 1), label = p.Project }));
    }
}
