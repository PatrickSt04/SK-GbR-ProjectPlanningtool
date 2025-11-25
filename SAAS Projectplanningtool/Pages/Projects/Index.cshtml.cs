using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.Models.IndependentTables;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class ProjectsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ProjectStatisticsCalculator _projectStatisticsCalculator;

        public ProjectsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _projectStatisticsCalculator = new ProjectStatisticsCalculator(_context, _userManager);
        }

        // Current User Data
        public Employee? CurrentEmployee { get; set; }
        public Company? CurrentCompany { get; set; }
        public string? CurrentUserRole { get; set; }

        // Projects Data
        public class ProjectWithBudget
        {
            public Project Project { get; set; }
            public double UsedBudget { get; set; }
        }
        public IEnumerable<ProjectWithBudget> Projects { get; set; } = new List<ProjectWithBudget>();

        // Statistics
        public int ActiveProjectsCount { get; set; }
        public int ArchivedProjectsCount { get; set; }
        public int UpcomingProjectsCount { get; set; }
        public int OverdueProjectsCount { get; set; }


        public int PlanningProjectsCount { get; set; }
        public int CompletedProjectsCount { get; set; }
        public int TotalProjectsCount { get; set; }

        // Filter Data
        public List<State> AvailableStatuses { get; set; } = new();
        public List<Customer> AvailableCustomers { get; set; } = new();
        public List<Employee> AvailableProjectManagers { get; set; } = new();

        // Filter Parameters
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CustomerFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ManagerFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DateFilter { get; set; }

        // Sorting Parameters
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "ProjectName";

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "asc";

        // Pagination Parameters
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 25;

        public int TotalPages { get; set; }

        // Permissions
        public bool CanCreateProjects { get; set; }
        public bool CanEditProjects { get; set; }
        public bool CanDeleteProjects { get; set; }

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

                CurrentEmployee = await _context.Employee
                    .Include(e => e.Company)
                    .Include(e => e.IdentityRole)
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

                // Load permissions
                await LoadPermissions();

                // Load filter data
                await LoadFilterData();

                // Load statistics
                await LoadStatistics();

                // Load projects with filters
                await LoadProjects();

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Projects Index Error: {ex.Message}");
                ViewData["ErrorMessage"] = "Fehler beim Laden der Projekte. Bitte versuchen Sie es später erneut.";
                return Page();
            }
        }

        private async Task LoadPermissions()
        {
            var userRole = CurrentEmployee?.IdentityRole?.Name;

            CanCreateProjects = userRole == "Admin" || userRole == "ProjectManager" || userRole == "Projektleiter";
            CanEditProjects = userRole == "Admin" || userRole == "ProjectManager" || userRole == "Projektleiter";
            CanDeleteProjects = userRole == "Admin";
        }

        private async Task LoadFilterData()
        {
            var companyId = CurrentCompany!.CompanyId;

            // Load available statuses
            AvailableStatuses = await _context.State
                .OrderBy(s => s.StateName)
                .ToListAsync();

            // Load available customers
            AvailableCustomers = await _context.Customer
                .Where(c => c.CompanyId == companyId && c.DeleteFlag == false)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            // Load available project managers
            AvailableProjectManagers = await _context.Employee
                .Where(e => e.CompanyId == companyId &&
                           e.DeleteFlag == false &&
                           (e.IdentityRole!.Name == "Admin" ||
                            e.IdentityRole.Name == "ProjectManager" ||
                            e.IdentityRole.Name == "Projektleiter"))
                .OrderBy(e => e.EmployeeDisplayName)
                .ToListAsync();
        }

        private async Task LoadStatistics()
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

        private async Task LoadProjects()
        {
            var companyId = CurrentCompany!.CompanyId;

            // Start with base query
            var query = _context.Project
                .Include(p => p.Customer)
                .Include(p => p.State)
                .Include(p => p.ResponsiblePerson)
                .Include(p => p.ProjectBudget)
                .Where(p => p.CompanyId == companyId);

            // Apply filters
            query = ApplyFilters(query);

            // Apply sorting
            query = ApplySorting(query);

            // Calculate total count for pagination
            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            // Ensure current page is valid
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

            // Apply pagination
            var tmp_Projects = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            Projects = new List<ProjectWithBudget>();
            foreach (var project in tmp_Projects)
            {
                double usedBudget = 0;
                if (project.ProjectBudget != null)
                {
                    usedBudget = _projectStatisticsCalculator.CalculateBudgetStatisticsAsync(project.ProjectId, User).Result.UsedBudget;
                }
                ((List<ProjectWithBudget>)Projects).Add(new ProjectWithBudget
                {
                    Project = project,
                    UsedBudget = usedBudget
                });
            }
        }

        private IQueryable<Project> ApplyFilters(IQueryable<Project> query)
        {
            // Search term filter
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var searchTermLower = SearchTerm.ToLower();
                query = query.Where(p =>
                    p.ProjectName.ToLower().Contains(searchTermLower) ||
                    p.ProjectDescription.ToLower().Contains(searchTermLower) ||
                    (p.Customer != null && p.Customer.CustomerName.ToLower().Contains(searchTermLower)));
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(StatusFilter))
            {
                query = query.Where(p => p.StateId == StatusFilter);
            }

            // Customer filter
            if (!string.IsNullOrWhiteSpace(CustomerFilter))
            {
                query = query.Where(p => p.CustomerId == CustomerFilter);
            }

            // Manager filter
            if (!string.IsNullOrWhiteSpace(ManagerFilter))
            {
                query = query.Where(p => p.ResponsiblePersonId == ManagerFilter);
            }

            // Date filter
            if (!string.IsNullOrWhiteSpace(DateFilter))
            {
                var today = DateTime.Now.Date;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var startOfYear = new DateTime(today.Year, 1, 1);
                var startOfLastYear = new DateTime(today.Year - 1, 1, 1);
                var endOfLastYear = new DateTime(today.Year - 1, 12, 31);

                query = DateFilter switch
                {
                    "thisMonth" => query.Where(p =>
                        (p.StartDate >= DateOnly.FromDateTime(startOfMonth) && p.StartDate <= DateOnly.FromDateTime(today)) ||
                        (p.EndDate >= DateOnly.FromDateTime(startOfMonth) && p.EndDate <= DateOnly.FromDateTime(today))),
                    "thisYear" => query.Where(p =>
                        (p.StartDate >= DateOnly.FromDateTime(startOfYear)) ||
                        (p.EndDate >= DateOnly.FromDateTime(startOfYear))),
                    "lastYear" => query.Where(p =>
                        (p.StartDate >= DateOnly.FromDateTime(startOfLastYear) && p.StartDate <= DateOnly.FromDateTime(endOfLastYear)) ||
                        (p.EndDate >= DateOnly.FromDateTime(startOfLastYear) && p.EndDate <= DateOnly.FromDateTime(endOfLastYear))),
                    _ => query
                };
            }

            return query;
        }

        private IQueryable<Project> ApplySorting(IQueryable<Project> query)
        {
            return SortBy switch
            {
                "ProjectName" => SortDirection == "asc" ?
                    query.OrderBy(p => p.ProjectName) :
                    query.OrderByDescending(p => p.ProjectName),
                "CustomerName" => SortDirection == "asc" ?
                    query.OrderBy(p => p.Customer!.CustomerName) :
                    query.OrderByDescending(p => p.Customer!.CustomerName),
                "StartDate" => SortDirection == "asc" ?
                    query.OrderBy(p => p.StartDate) :
                    query.OrderByDescending(p => p.StartDate),
                "EndDate" => SortDirection == "asc" ?
                    query.OrderBy(p => p.EndDate) :
                    query.OrderByDescending(p => p.EndDate),
                "Status" => SortDirection == "asc" ?
                    query.OrderBy(p => p.State!.StateName) :
                    query.OrderByDescending(p => p.State!.StateName),
                "ResponsiblePerson" => SortDirection == "asc" ?
                    query.OrderBy(p => p.ResponsiblePerson!.EmployeeDisplayName) :
                    query.OrderByDescending(p => p.ResponsiblePerson!.EmployeeDisplayName),
                _ => query.OrderBy(p => p.ProjectName)
            };
        }

        public Dictionary<string, string> GetRouteData()
        {
            var routeData = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                routeData.Add("searchTerm", SearchTerm);
            if (!string.IsNullOrWhiteSpace(StatusFilter))
                routeData.Add("statusFilter", StatusFilter);
            if (!string.IsNullOrWhiteSpace(CustomerFilter))
                routeData.Add("customerFilter", CustomerFilter);
            if (!string.IsNullOrWhiteSpace(ManagerFilter))
                routeData.Add("managerFilter", ManagerFilter);
            if (!string.IsNullOrWhiteSpace(DateFilter))
                routeData.Add("dateFilter", DateFilter);
            if (SortBy != "ProjectName")
                routeData.Add("sortBy", SortBy);
            if (SortDirection != "asc")
                routeData.Add("sortDirection", SortDirection);
            if (PageSize != 25)
                routeData.Add("pageSize", PageSize.ToString());

            return routeData;
        }

        // AJAX Actions for Project Management
        public async Task<IActionResult> OnPostArchiveAsync([FromBody] ArchiveProjectRequest request)
        {
            try
            {
                var project = await _context.Project
                    .FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId &&
                                            p.CompanyId == CurrentCompany!.CompanyId);

                if (project == null)
                {
                    return NotFound();
                }

                project.IsArchived = true;
                project.LatestModificationTimestamp = DateTime.Now;
                project.LatestModifierId = CurrentEmployee!.EmployeeId;
                project.LatestModificationText = "Projekt archiviert";

                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Archive Project Error: {ex.Message}");
                return new JsonResult(new { success = false, error = "Fehler beim Archivieren des Projekts." });
            }
        }

        public async Task<IActionResult> OnPostUnarchiveAsync([FromBody] ArchiveProjectRequest request)
        {
            try
            {
                var project = await _context.Project
                    .FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId &&
                                            p.CompanyId == CurrentCompany!.CompanyId);

                if (project == null)
                {
                    return NotFound();
                }

                project.IsArchived = false;
                project.LatestModificationTimestamp = DateTime.Now;
                project.LatestModifierId = CurrentEmployee!.EmployeeId;
                project.LatestModificationText = "Projekt reaktiviert";

                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unarchive Project Error: {ex.Message}");
                return new JsonResult(new { success = false, error = "Fehler beim Reaktivieren des Projekts." });
            }
        }

        public async Task<IActionResult> OnGetExportAsync(string export)
        {
            try
            {
                // Load all projects with current filters (without pagination)
                var companyId = CurrentCompany!.CompanyId;
                var query = _context.Project
                    .Include(p => p.Customer)
                    .Include(p => p.State)
                    .Include(p => p.ResponsiblePerson)
                    .Include(p => p.ProjectBudget)
                    .Where(p => p.CompanyId == companyId);

                query = ApplyFilters(query);
                query = ApplySorting(query);

                var projects = await query.ToListAsync();

                return export switch
                {
                    "excel" => await ExportToExcel(projects),
                    "pdf" => await ExportToPdf(projects),
                    "csv" => await ExportToCsv(projects),
                    _ => BadRequest("Ungültiges Export-Format")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Export Error: {ex.Message}");
                return BadRequest("Fehler beim Exportieren der Daten.");
            }
        }

        private async Task<IActionResult> ExportToExcel(List<Project> projects)
        {
            // Implementierung für Excel-Export
            // Hier würden Sie eine Excel-Bibliothek wie EPPlus verwenden
            return BadRequest("Excel-Export noch nicht implementiert");
        }

        private async Task<IActionResult> ExportToPdf(List<Project> projects)
        {
            // Implementierung für PDF-Export
            // Hier würden Sie eine PDF-Bibliothek wie iTextSharp verwenden
            return BadRequest("PDF-Export noch nicht implementiert");
        }

        private async Task<IActionResult> ExportToCsv(List<Project> projects)
        {
            // Implementierung für PDF-Export
            // Hier würden Sie eine PDF-Bibliothek wie iTextSharp verwenden
            return BadRequest("CSV-Export noch nicht implementiert");

        }

        // Helper classes for AJAX requests
        public class ArchiveProjectRequest
        {
            public string ProjectId { get; set; } = string.Empty;
        }
    }

    // Extension methods for better filtering
    public static class ProjectQueryExtensions
    {
        public static IQueryable<Project> FilterBySearchTerm(this IQueryable<Project> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var searchTermLower = searchTerm.ToLower();
            return query.Where(p =>
                p.ProjectName.ToLower().Contains(searchTermLower) ||
                p.ProjectDescription.ToLower().Contains(searchTermLower) ||
                (p.Customer != null && p.Customer.CustomerName.ToLower().Contains(searchTermLower)));
        }

        public static IQueryable<Project> FilterByStatus(this IQueryable<Project> query, string? statusId)
        {
            if (string.IsNullOrWhiteSpace(statusId))
                return query;

            return query.Where(p => p.StateId == statusId);
        }

        public static IQueryable<Project> FilterByCustomer(this IQueryable<Project> query, string? customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                return query;

            return query.Where(p => p.CustomerId == customerId);
        }

        public static IQueryable<Project> FilterByManager(this IQueryable<Project> query, string? managerId)
        {
            if (string.IsNullOrWhiteSpace(managerId))
                return query;

            return query.Where(p => p.ResponsiblePersonId == managerId);
        }

        public static IQueryable<Project> FilterByDateRange(this IQueryable<Project> query, string? dateFilter)
        {
            if (string.IsNullOrWhiteSpace(dateFilter))
                return query;

            var today = DateTime.Now.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);
            var startOfLastYear = new DateTime(today.Year - 1, 1, 1);
            var endOfLastYear = new DateTime(today.Year - 1, 12, 31);

            return dateFilter switch
            {
                "thisMonth" => query.Where(p =>
                    (p.StartDate >= DateOnly.FromDateTime(startOfMonth) && p.StartDate <= DateOnly.FromDateTime(today)) ||
                    (p.EndDate >= DateOnly.FromDateTime(startOfMonth) && p.EndDate <= DateOnly.FromDateTime(today))),
                "thisYear" => query.Where(p =>
                    (p.StartDate >= DateOnly.FromDateTime(startOfYear)) ||
                    (p.EndDate >= DateOnly.FromDateTime(startOfYear))),
                "lastYear" => query.Where(p =>
                    (p.StartDate >= DateOnly.FromDateTime(startOfLastYear) && p.StartDate <= DateOnly.FromDateTime(endOfLastYear)) ||
                    (p.EndDate >= DateOnly.FromDateTime(startOfLastYear) && p.EndDate <= DateOnly.FromDateTime(endOfLastYear))),
                _ => query
            };
        }

        public static IQueryable<Project> ApplyProjectSorting(this IQueryable<Project> query, string sortBy, string sortDirection)
        {
            return sortBy switch
            {
                "ProjectName" => sortDirection == "asc" ?
                    query.OrderBy(p => p.ProjectName) :
                    query.OrderByDescending(p => p.ProjectName),
                "CustomerName" => sortDirection == "asc" ?
                    query.OrderBy(p => p.Customer!.CustomerName) :
                    query.OrderByDescending(p => p.Customer!.CustomerName),
                "StartDate" => sortDirection == "asc" ?
                    query.OrderBy(p => p.StartDate) :
                    query.OrderByDescending(p => p.StartDate),
                "EndDate" => sortDirection == "asc" ?
                    query.OrderBy(p => p.EndDate) :
                    query.OrderByDescending(p => p.EndDate),
                "Status" => sortDirection == "asc" ?
                    query.OrderBy(p => p.State!.StateName) :
                    query.OrderByDescending(p => p.State!.StateName),
                "ResponsiblePerson" => sortDirection == "asc" ?
                    query.OrderBy(p => p.ResponsiblePerson!.EmployeeDisplayName) :
                    query.OrderByDescending(p => p.ResponsiblePerson!.EmployeeDisplayName),
                _ => query.OrderBy(p => p.ProjectName)
            };
        }
    }
}