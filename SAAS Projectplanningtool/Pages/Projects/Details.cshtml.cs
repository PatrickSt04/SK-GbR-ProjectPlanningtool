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

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class DetailsModel : ProjectPageModel
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

        public Project Project { get; set; } = default!;
        public int TotalTasks { get; set; } = 0;
        public int CompletetedTasks { get; set; } = 0;
        public async Task<IActionResult> OnGetAsync(string id)
        {
            await _logger.Log(null, User, null, "Projects/Details<OnGet>Beginn");
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.LatestModifier)
                .Include(p => p.Company)
                .Include(p => p.Customer)
                .Include(p => p.ProjectBudget)
                .Include(p => p.ResponsiblePerson)
                .Include(p => p.State)
                // Projectsections lesen
                .Include(p => p.ProjectSections)
                //deren Tasks
                .ThenInclude(ps => ps.ProjectTasks)
                .ThenInclude(pt => pt.State)
                // Projectsections lesen
                .Include(p => p.ProjectSections)
                // deren Subsections    
                .ThenInclude(ps => ps.SubSections)
                //deren Tasks
                .ThenInclude(ss => ss.ProjectTasks)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                Project = project;
            }
            Employee employee = default!;
            try
            {
                employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, employee, null) });
            }
            var sectionsOfThisProject = await _context.ProjectSection
                 .Where(s => s.ProjectId == id)
                 .Where(s => s.CompanyId == employee.CompanyId)
                 .Select(s => s.ProjectSectionId)
                 .ToListAsync();
            try
            {

                TotalTasks = await _context.ProjectTask
                    .Where(pt => sectionsOfThisProject.Contains(pt.ProjectSectionId))
                    .Where(pt => pt.CompanyId == employee.CompanyId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, TotalTasks, null) });
            }

            var completedState = await _context.State.FirstOrDefaultAsync(s => s.StateName == "Abgeschlossen");
            try
            {
                CompletetedTasks = await _context.ProjectTask
                    .Where(pt => sectionsOfThisProject.Contains(pt.ProjectSectionId))
                    .Where(pt => pt.CompanyId == employee.CompanyId)
                    .Where(pt => pt.StateId == completedState.StateId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, CompletetedTasks, null) });
            }
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
    }
}
