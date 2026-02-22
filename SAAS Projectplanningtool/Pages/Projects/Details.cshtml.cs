using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.CustomManagers;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis;
using Project = SAAS_Projectplanningtool.Models.Budgetplanning.Project;
using SAAS_Projectplanningtool.Models.TimeTracking;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class DetailsModel : ProjectHandlerPageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        public DetailsModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }
        [BindProperty]
        public List<ProjectTaskCatalogTask> TaskCatalog { get; set; } = new List<ProjectTaskCatalogTask>();

        //Information for Project progress
        public int TotalTasks { get; set; } = 0;
        public int CompletetedTasks { get; set; } = 0;

        public double ProgressInDecimals { get; set; } = 0.0;

        // showCompleted refers to the task catalog (include or exclude completed tasks)

        [BindProperty(SupportsGet = true)]
        public bool showCompleted { get; set; } = default!;


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



        public bool ScheduleAlreadyExists { get; set; } = false;
        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/Details<OnGet>Beginn");
                if (id == null)
                {
                    return NotFound();
                }
                await SetProjectBindingAsync(id);
                if (Project == null)
                {
                    return NotFound();
                }

                var employee = await GetEmployeeAsync();

                if (employee?.CompanyId != null)
                {
                    await SetHolidaysBindingAsync(employee.CompanyId);
                }
                TaskCatalog = Project.ProjectTaskCatalogTasks != null ? Project.ProjectTaskCatalogTasks.ToList() : new List<ProjectTaskCatalogTask>();


                //Projects statistics
                var projectStats = await new ProjectStatisticsCalculator(_context, _userManager).CalculateStatisticsAsync(Project.ProjectId, User);
                TotalTasks = projectStats.TotalTasks;
                CompletetedTasks = projectStats.CompletedTasks;
                ProgressInDecimals = projectStats.Progress;


                if (Project.ProjectSections != null)
                {
                    //Es existiert ein Terminplan
                    ScheduleAlreadyExists = true;
                    foreach (var section in Project.ProjectSections)
                    {
                        section.State = await new StateManager(_context).GetSectionState(section.ProjectSectionId);

                        if (section.SubSections != null)
                        {
                            foreach (var subsection in section.SubSections)
                            {
                                subsection.State = await new StateManager(_context).GetSectionState(subsection.ProjectSectionId);
                            }
                        }
                    }
                }

                ViewData["AllStates"] = await _context.State.ToListAsync();


                #region Time Tracking: Mitarbeiter- und Zeiteinträge laden

                CurrentEmployeeId = employee?.EmployeeId;

                // Rolle ermitteln
                if (employee?.IdentityRole != null)
                {
                    IsWorkerRole = employee.IdentityRole.Name == "Worker";
                }
                else if (employee != null)
                {
                    var role = await _context.Roles
                        .FirstOrDefaultAsync(r => r.Id == employee.IdentityRoleId);
                    IsWorkerRole = role?.Name == "Worker";
                }

                // Mitarbeiter-Liste laden (nur für Nicht-Worker, für das Dropdown)
                if (!IsWorkerRole && employee?.CompanyId != null)
                {
                    CompanyEmployees = await _context.Employee
                        .Where(e => e.CompanyId == employee.CompanyId)
                        .Where(e => e.DeleteFlag == false)
                        .OrderBy(e => e.EmployeeDisplayName)
                        .ToListAsync();
                }

                // Zeiteinträge mit Pagination laden
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

                // Gesamtstunden berechnen (über ALLE Einträge, nicht nur aktuelle Seite)
                var allTimeEntries = await _context.TimeEntry
                    .Where(t => t.ProjectId == id)
                    .Where(t => t.CompanyId == employee.CompanyId)
                    .ToListAsync();

                TotalProjectHours = allTimeEntries.Sum(t => t.NetWorkingHours);

                #endregion



                await _logger.Log(null, User, null, "Projects/Details<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Project, "Projects / Details < OnGet > ERROR") });
            }
        }
        public async Task<IActionResult> OnPostToggleTaskStateAsync(string? ProjectTaskId, string? projectId)
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/Details<OnPostToggleTaskStateAsync>Begin");
                // Hier: Aufgabenstatus ändern im DB-Kontext
                var task = await _context.ProjectTaskCatalogTask.FindAsync(ProjectTaskId);
                if (task != null)
                {
                    var openState = await _context.State.FirstOrDefaultAsync(s => s.StateName == "Offen");
                    var doneState = await _context.State.FirstOrDefaultAsync(s => s.StateName == "Abgeschlossen");

                    task.StateId = task.StateId == openState.StateId ? doneState.StateId : openState.StateId;
                    _context.ProjectTaskCatalogTask.Update(task);
                    await _context.SaveChangesAsync();
                }
                await _logger.Log(null, User, null, "Projects/Details<OnPostToggleTaskStateAsync>End");
                return RedirectToPage(new { id = projectId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, ProjectTaskId, "Projects/Details<OnPostToggleTaskStateAsync>End") });
            }
        }

        public async Task<IActionResult> OnPostUpdateTaskStateAsync(string taskId, string stateId, string projectId)
        {
            var task = await _context.ProjectTask.FindAsync(taskId);
            if (task != null)
            {
                task.StateId = stateId;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id = projectId });
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

                // Rolle prüfen
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == currentEmployee.IdentityRoleId);
                bool isWorker = role?.Name == "Worker";

                // Ziel-Mitarbeiter bestimmen
                string targetEmployeeId;
                if (isWorker)
                {
                    // Worker dürfen nur für sich selbst erfassen
                    targetEmployeeId = currentEmployee.EmployeeId;
                }
                else
                {
                    // Andere Rollen: ausgewählten Mitarbeiter oder sich selbst
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
        /// Zeiteintrag löschen.
        /// Worker: nur eigene Einträge. Andere Rollen: alle Einträge.
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

                // Berechtigungsprüfung: Worker dürfen nur eigene Einträge löschen
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == currentEmployee.IdentityRoleId);

                if (role?.Name == "Worker" && entry.EmployeeId != currentEmployee.EmployeeId)
                {
                    TempData.SetMessage("Error", "Sie können nur eigene Zeiteinträge löschen.");
                    return RedirectToPage(new { id = projectId, timeTrackingPage });
                }

                _context.TimeEntry.Remove(entry);
                await _context.SaveChangesAsync();

                TempData.SetMessage("Success", "Zeiteintrag wurde gelöscht.");
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
