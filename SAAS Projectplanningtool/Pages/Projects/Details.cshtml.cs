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
        public List<ProjectTask> TaskCatalog { get; set; } = new List<ProjectTask>();

        //Information for Project progress
        public int TotalTasks { get; set; } = 0;
        public int CompletetedTasks { get; set; } = 0;

        public double ProgressInDecimals { get; set; } = 0.0;

        // showCompleted refers to the task catalog (include or exclude completed tasks)

        [BindProperty(SupportsGet = true)]
        public bool showCompleted { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/Details<OnGet>Beginn");
                if (id == null)
                {
                    return NotFound();
                }
                var project = await GetProjectAsync(id);
                if (project == null)
                {
                    return NotFound();
                }
                else
                {
                    Project = project;
                }
                Employee employee = default!;

                employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));

                var sectionsOfThisProject = await _context.ProjectSection
                     .Where(s => s.ProjectId == id)
                     .Where(s => s.CompanyId == employee.CompanyId)
                     .Select(s => s.ProjectSectionId)
                     .ToListAsync();


                //TaskCatalog
                var sectionsofProject = await _context.ProjectSection
                    .Where(ps => ps.ProjectId == id)
                    .Where(ps => ps.CompanyId == employee.CompanyId)
                    .Select(ps => ps.ProjectSectionId)
                    .ToListAsync();
                TaskCatalog = await _context.ProjectTask.Where(pt => pt.CompanyId == employee.CompanyId)
                        .Where(pt => pt.IsTaskCatalogEntry == true)
                        .Where(pt => sectionsofProject.Contains(pt.ProjectSectionId))
                        .ToListAsync();

                //Projects statistics
                var projectStats = await new ProjectStatisticsCalculator(_context, _userManager).CalculateStatisticsAsync(Project.ProjectId, User);
                TotalTasks = projectStats.TotalTasks;
                CompletetedTasks = projectStats.CompletedTasks;
                ProgressInDecimals = projectStats.Progress;


                if (Project.ProjectSections != null)
                {
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
                var task = await _context.ProjectTask.FindAsync(ProjectTaskId);
                if (task != null)
                {
                    var openState = await _context.State.FirstOrDefaultAsync(s => s.StateName == "Offen");
                    var doneState = await _context.State.FirstOrDefaultAsync(s => s.StateName == "Abgeschlossen");

                    task.StateId = task.StateId == openState.StateId ? doneState.StateId : openState.StateId;
                    _context.ProjectTask.Update(task);
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

    }
}
