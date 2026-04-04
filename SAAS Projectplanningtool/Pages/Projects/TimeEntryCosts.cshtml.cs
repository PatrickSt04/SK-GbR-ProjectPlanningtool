using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.Models.TimeTracking;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class TimeEntryCostsModel : ProjectPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ProjectStatisticsCalculator _statisticsCalculator;

        public TimeEntryCostsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
            : base(context, userManager, roleManager)
        {
            _context = context;
            _userManager = userManager;
            _statisticsCalculator = new ProjectStatisticsCalculator(context, userManager);
        }

        public List<TimeEntryWithCost> TimeEntriesWithCosts { get; set; } = new();
        public double TotalCosts { get; set; }
        public double TotalHours { get; set; }

        // Gruppiert nach Stundensatzgruppe
        public List<HRGSummary> HRGSummaries { get; set; } = new();

        public class TimeEntryWithCost
        {
            public string TimeEntryId { get; set; } = string.Empty;
            public string EmployeeName { get; set; } = string.Empty;
            public string HourlyRateGroupName { get; set; } = string.Empty;
            public decimal HourlyRate { get; set; }
            public DateOnly WorkDate { get; set; }
            public TimeOnly StartTime { get; set; }
            public TimeOnly EndTime { get; set; }
            public int BreakMinutes { get; set; }
            public double NetWorkingHours { get; set; }
            public double Costs { get; set; }
            public string? Description { get; set; }
        }

        public class HRGSummary
        {
            public string HourlyRateGroupName { get; set; } = string.Empty;
            public decimal HourlyRate { get; set; }
            public double TotalHours { get; set; }
            public double TotalCosts { get; set; }
            public int EntryCount { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                    return NotFound();

                await SetProjectBindingAsync(id);
                if (Project == null)
                    return NotFound();

                var employee = await GetEmployeeAsync();
                if (employee?.CompanyId == null)
                    return NotFound();

                var timeEntries = await _context.TimeEntry
                    .Include(t => t.Employee)
                        .ThenInclude(e => e.HourlyRateGroup)
                    .Where(t => t.ProjectId == id)
                    .Where(t => t.CompanyId == employee.CompanyId)
                    .OrderByDescending(t => t.WorkDate)
                    .ThenByDescending(t => t.StartTime)
                    .ToListAsync();
                var projectHourlyRates = await _statisticsCalculator.GetProjectHourlyRateLookupAsync(id);
                TimeEntriesWithCosts = timeEntries.Select(t => 
                {
                    //var rate = (decimal)(t.Employee?.HourlyRateGroup?.HourlyRate ?? 0);
                    // Neu
            
                    var rate = _statisticsCalculator.ResolveEffectiveHourlyRate(
                        t.Employee?.HourlyRateGroup?.HourlyRateGroupId,
                        projectHourlyRates,
                        t.Employee?.HourlyRateGroup?.HourlyRate);

                    return new TimeEntryWithCost
                    {
                        TimeEntryId = t.TimeEntryId,
                        EmployeeName = t.Employee?.EmployeeDisplayName ?? "Unbekannt",
                        HourlyRateGroupName = t.Employee?.HourlyRateGroup?.HourlyRateGroupName ?? "Ohne Gruppe",
                        HourlyRate = rate,
                        WorkDate = t.WorkDate,
                        StartTime = t.StartTime,
                        EndTime = t.EndTime,
                        BreakMinutes = t.BreakMinutes,
                        NetWorkingHours = t.NetWorkingHours,
                        Costs = t.NetWorkingHours * (double)rate,
                        Description = t.Description
                    };
                }).ToList();

                TotalHours = TimeEntriesWithCosts.Sum(t => t.NetWorkingHours);
                TotalCosts = TimeEntriesWithCosts.Sum(t => t.Costs);

                HRGSummaries = TimeEntriesWithCosts
                    .GroupBy(t => new { t.HourlyRateGroupName, t.HourlyRate })
                    .Select(g => new HRGSummary
                    {
                        HourlyRateGroupName = g.Key.HourlyRateGroupName,
                        HourlyRate = g.Key.HourlyRate,
                        TotalHours = g.Sum(x => x.NetWorkingHours),
                        TotalCosts = g.Sum(x => x.Costs),
                        EntryCount = g.Count()
                    })
                    .OrderByDescending(g => g.TotalCosts)
                    .ToList();

                return Page();
            }
            catch (Exception ex)
            {
                //TODO: Log error
            }
            return Page();
        }

       
    }
}