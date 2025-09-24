using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Pages
{
    public class Index1Model : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public Index1Model(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Current User Data
        public Employee? CurrentEmployee { get; set; }
        public Company? CurrentCompany { get; set; }
        public string? CurrentUserRole { get; set; }

        // Project Statistics
        public int ActiveProjectsCount { get; set; }
        public int ArchivedProjectsCount { get; set; }
        public int UpcomingProjectsCount { get; set; }
        public int OverdueProjectsCount { get; set; }
        public int TotalProjectsCount { get; set; }

        // Budget Data
        public List<Project> ProjectsWithBudget { get; set; } = new();
        public List<Project> HighBudgetProjects { get; set; } = new();

        // Task Data
        public List<ProjectTask> MyOpenTasks { get; set; } = new();
        public List<ProjectTask> TeamTasks { get; set; } = new();
        public int TotalTasksCount { get; set; }

        // Employee Workload
        public List<EmployeeWorkloadDto> EmployeeWorkload { get; set; } = new();
        public decimal AverageHourlyRate { get; set; }
        public decimal HighestHourlyRate { get; set; }
        public decimal LowestHourlyRate { get; set; }

        // Recent Activities
        public List<ActivityDto> RecentActivities { get; set; } = new();

        // Permissions
        public bool CanCreateProjects { get; set; }
        public bool CanCreateTasks { get; set; }
        public bool CanCreateCustomers { get; set; }
        public bool CanCreateEmployees { get; set; }

        // Calendar Events
        public List<CalendarEventDto> ThisWeekEvents { get; set; } = new();

        // Customer Data
        public List<Customer> RecentCustomers { get; set; } = new();

        // System Info
        public int ActiveUsersCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get current user and employee
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToPage("/Account/Login");
                }

                // Korrigierte DbSet-Namen (singular)
                CurrentEmployee = await _context.Employee
                    .Include(e => e.Company)
                    .Include(e => e.IdentityRole)
                    .Include(e => e.HourlyRateGroup)
                    .FirstOrDefaultAsync(e => e.IdentityUserId == currentUser.Id);

                if (CurrentEmployee == null)
                {
                    return RedirectToPage("/Setup/EmployeeSetup");
                }

                CurrentCompany = CurrentEmployee.Company;
                CurrentUserRole = CurrentEmployee.IdentityRole?.Name;

                if (CurrentCompany == null)
                {
                    return RedirectToPage("/Setup/CompanySetup");
                }

                // Load all dashboard data
                await LoadProjectStatistics();
                await LoadBudgetData();
                await LoadTaskData();
                await LoadEmployeeWorkload();
                await LoadRecentActivities();
                await LoadPermissions();
                await LoadCalendarEvents();
                await LoadRecentCustomers();
                await LoadSystemInfo();

                return Page();
            }
            catch (Exception ex)
            {
                // Log error - in Production sollten Sie ein echtes Logging-Framework verwenden
                Console.WriteLine($"Dashboard Error: {ex.Message}");

                // Optional: ViewData für Fehlermeldung setzen
                ViewData["ErrorMessage"] = "Fehler beim Laden des Dashboards. Bitte versuchen Sie es später erneut.";

                return Page(); // Show page with empty data
            }
        }

        private async Task LoadProjectStatistics()
        {
            var companyId = CurrentCompany!.CompanyId;
            var today = DateOnly.FromDateTime(DateTime.Now);
            var thirtyDaysFromNow = today.AddDays(30);

            // Active projects
            ActiveProjectsCount = await _context.Project
                .CountAsync(p => p.CompanyId == companyId && p.IsArchived != true);

            // Archived projects
            ArchivedProjectsCount = await _context.Project
                .CountAsync(p => p.CompanyId == companyId && p.IsArchived == true);

            // Upcoming projects (starting in next 30 days)
            UpcomingProjectsCount = await _context.Project
                .CountAsync(p => p.CompanyId == companyId &&
                           p.StartDate >= today &&
                           p.StartDate <= thirtyDaysFromNow &&
                           p.IsArchived != true);

            // Overdue projects
            OverdueProjectsCount = await _context.Project
                .CountAsync(p => p.CompanyId == companyId &&
                           p.EndDate < today &&
                           p.IsArchived != true);

            // Total projects
            TotalProjectsCount = await _context.Project
                .CountAsync(p => p.CompanyId == companyId);
        }

        private async Task LoadBudgetData()
        {
            var companyId = CurrentCompany!.CompanyId;

            // Projects with budget information
            ProjectsWithBudget = await _context.Project
                .Include(p => p.ProjectBudget)
                .Where(p => p.CompanyId == companyId &&
                           p.IsArchived != true &&
                           p.ProjectBudget != null)
                .Take(10)
                .ToListAsync();

            // High budget projects (>90% used)
            HighBudgetProjects = ProjectsWithBudget
                .Where(p => p.ProjectBudget != null &&
                           p.ProjectBudget.InitialBudget > 0 &&
                           (p.ProjectBudget.UsedBudget / p.ProjectBudget.InitialBudget) > 0.9f)
                .ToList();
        }

        private async Task LoadTaskData()
        {
            var companyId = CurrentCompany!.CompanyId;
            var currentEmployeeId = CurrentEmployee!.EmployeeId;

            // My open tasks - Korrigierte DbSet-Namen
            MyOpenTasks = await _context.ProjectTask
                .Include(t => t.ProjectSection)
                    .ThenInclude(ps => ps!.Project)
                .Include(t => t.State)
                .OrderBy(t => t.EndDate)
                .Take(10)
                .ToListAsync();

            // Team tasks (all open tasks in company)
            TeamTasks = await _context.ProjectTask
                .Include(t => t.ProjectSection)
                    .ThenInclude(ps => ps!.Project)
                .Include(t => t.State)
                .Where(t => t.CompanyId == companyId &&
                           t.State != null &&
                           t.State.StateName != "Completed")
                .OrderBy(t => t.EndDate)
                .Take(20)
                .ToListAsync();

            // Total tasks count
            TotalTasksCount = await _context.ProjectTask
                .CountAsync(t => t.CompanyId == companyId);
        }

        private async Task LoadEmployeeWorkload()
        {
            var companyId = CurrentCompany!.CompanyId;

            // Get employee workload data
            var employees = await _context.Employee
                .Include(e => e.HourlyRateGroup)
                .Where(e => e.CompanyId == companyId && e.DeleteFlag == false)
                .ToListAsync();

            EmployeeWorkload = new List<EmployeeWorkloadDto>();

            //foreach (var employee in employees)
            //{
            //    var taskCount = await _context.ProjectTask
            //        .Include(t => t.ProjectTaskRessource)
            //            .ThenInclude(ptr => ptr!.EmployeeRessources)
            //        .CountAsync(t => t.CompanyId == companyId &&
            //                   t.ProjectTaskRessource != null &&
            //                   t.ProjectTaskRessource.EmployeeRessources != null &&
            //                   t.ProjectTaskRessource.EmployeeRessources.Any(e => e.EmployeeId == employee.EmployeeId) &&
            //                   t.State != null &&
            //                   t.State.StateName != "Completed");

            //    EmployeeWorkload.Add(new EmployeeWorkloadDto
            //    {
            //        EmployeeId = employee.EmployeeId,
            //        EmployeeDisplayName = employee.EmployeeDisplayName,
            //        TaskCount = taskCount,
            //        HourlyRateGroup = employee.HourlyRateGroup
            //    });
            //}

            // Calculate hourly rate statistics
            var hourlyRates = await _context.HourlyRateGroup
                .Where(hrg => hrg.CompanyId == companyId && hrg.DeleteFlag == false)
                .Select(hrg => (decimal)hrg.HourlyRate)
                .ToListAsync();

            if (hourlyRates.Any())
            {
                AverageHourlyRate = hourlyRates.Average();
                HighestHourlyRate = hourlyRates.Max();
                LowestHourlyRate = hourlyRates.Min();
            }
        }

        private async Task LoadRecentActivities()
        {
            var companyId = CurrentCompany!.CompanyId;
            var activities = new List<ActivityDto>();

            // Recent project modifications
            var recentProjects = await _context.Project
                .Include(p => p.LatestModifier)
                .Where(p => p.CompanyId == companyId && p.LatestModificationTimestamp != null)
                .OrderByDescending(p => p.LatestModificationTimestamp)
                .Take(5)
                .ToListAsync();

            foreach (var project in recentProjects)
            {
                activities.Add(new ActivityDto
                {
                    Description = $"Projekt '{project.ProjectName}' wurde aktualisiert: {project.LatestModificationText}",
                    ModifierName = project.LatestModifier?.EmployeeDisplayName ?? "System",
                    Timestamp = project.LatestModificationTimestamp ?? DateTime.Now,
                    Icon = "fa-project-diagram",
                    Type = "Project"
                });
            }

            // Recent task modifications
            var recentTasks = await _context.ProjectTask
                .Include(t => t.LatestModifier)
                .Where(t => t.CompanyId == companyId && t.LatestModificationTimestamp != null)
                .OrderByDescending(t => t.LatestModificationTimestamp)
                .Take(5)
                .ToListAsync();

            foreach (var task in recentTasks)
            {
                activities.Add(new ActivityDto
                {
                    Description = $"Aufgabe '{task.ProjectTaskName}' wurde aktualisiert: {task.LatestModificationText}",
                    ModifierName = task.LatestModifier?.EmployeeDisplayName ?? "System",
                    Timestamp = task.LatestModificationTimestamp ?? DateTime.Now,
                    Icon = "fa-tasks",
                    Type = "Task"
                });
            }

            // Recent customer additions
            var recentCustomers = await _context.Customer
                .Include(c => c.CreatedByEmployee)
                .Where(c => c.CompanyId == companyId && c.CreatedTimestamp != null)
                .OrderByDescending(c => c.CreatedTimestamp)
                .Take(3)
                .ToListAsync();

            foreach (var customer in recentCustomers)
            {
                activities.Add(new ActivityDto
                {
                    Description = $"Neuer Kunde '{customer.CustomerName}' wurde hinzugefügt",
                    ModifierName = customer.CreatedByEmployee?.EmployeeDisplayName ?? "System",
                    Timestamp = customer.CreatedTimestamp ?? DateTime.Now,
                    Icon = "fa-user-plus",
                    Type = "Customer"
                });
            }

            RecentActivities = activities
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToList();
        }

        private async Task LoadPermissions()
        {
            var userRole = CurrentEmployee?.IdentityRole?.Name;

            // Set permissions based on role
            CanCreateProjects = userRole == "Admin" || userRole == "ProjectManager" || userRole == "Projektleiter";
            CanCreateTasks = userRole == "Admin" || userRole == "ProjectManager" || userRole == "Projektleiter" || userRole == "TeamLead";
            CanCreateCustomers = userRole == "Admin" || userRole == "ProjectManager" || userRole == "Projektleiter" || userRole == "Sales";
            CanCreateEmployees = userRole == "Admin" || userRole == "HR";

            await Task.CompletedTask; // Async placeholder
        }

        private async Task LoadCalendarEvents()
        {
            var companyId = CurrentCompany!.CompanyId;
            var today = DateTime.Now.Date;
            var nextWeek = today.AddDays(7);

            var events = new List<CalendarEventDto>();

            // Project start dates this week
            var projectStarts = await _context.Project
                .Where(p => p.CompanyId == companyId &&
                           p.StartDate >= DateOnly.FromDateTime(today) &&
                           p.StartDate <= DateOnly.FromDateTime(nextWeek))
                .ToListAsync();

            foreach (var project in projectStarts)
            {
                if (project.StartDate.HasValue)
                {
                    events.Add(new CalendarEventDto
                    {
                        Date = project.StartDate.Value.ToDateTime(TimeOnly.MinValue),
                        Title = $"Projektstart: {project.ProjectName}",
                        Type = "Projekt"
                    });
                }
            }

            // Project end dates this week
            var projectEnds = await _context.Project
                .Where(p => p.CompanyId == companyId &&
                           p.EndDate >= DateOnly.FromDateTime(today) &&
                           p.EndDate <= DateOnly.FromDateTime(nextWeek))
                .ToListAsync();

            foreach (var project in projectEnds)
            {
                if (project.EndDate.HasValue)
                {
                    events.Add(new CalendarEventDto
                    {
                        Date = project.EndDate.Value.ToDateTime(TimeOnly.MinValue),
                        Title = $"Projektende: {project.ProjectName}",
                        Type = "Projekt"
                    });
                }
            }

            // Task deadlines this week
            var taskDeadlines = await _context.ProjectTask
                .Where(t => t.CompanyId == companyId &&
                           t.EndDate >= DateOnly.FromDateTime(today) &&
                           t.EndDate <= DateOnly.FromDateTime(nextWeek))
                .ToListAsync();

            foreach (var task in taskDeadlines)
            {
                if (task.EndDate.HasValue)
                {
                    events.Add(new CalendarEventDto
                    {
                        Date = task.EndDate.Value.ToDateTime(TimeOnly.MinValue),
                        Title = $"Aufgabe fällig: {task.ProjectTaskName}",
                        Type = "Aufgabe"
                    });
                }
            }

            ThisWeekEvents = events.OrderBy(e => e.Date).ToList();
        }

        private async Task LoadRecentCustomers()
        {
            var companyId = CurrentCompany!.CompanyId;

            RecentCustomers = await _context.Customer
                .Include(c => c.Address)
                .Where(c => c.CompanyId == companyId && c.DeleteFlag == false)
                .OrderByDescending(c => c.CreatedTimestamp)
                .Take(5)
                .ToListAsync();
        }

        private async Task LoadSystemInfo()
        {
            var companyId = CurrentCompany!.CompanyId;

            // Count active users/employees
            ActiveUsersCount = await _context.Employee
                .CountAsync(e => e.CompanyId == companyId && e.DeleteFlag == false);
        }
    }

    // DTOs for dashboard data
    public class EmployeeWorkloadDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeDisplayName { get; set; } = string.Empty;
        public int TaskCount { get; set; }
        public HourlyRateGroup? HourlyRateGroup { get; set; }
    }

    public class ActivityDto
    {
        public string Description { get; set; } = string.Empty;
        public string ModifierName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class CalendarEventDto
    {
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}