using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    // This file is used for editing the schedule of a project.
    public class EditModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public EditModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
  
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await _logger.Log(null, User, null, "Projects.Scheduling<OnGetAsync>Begin");
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
                Project = project;
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnGetAsync>End");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostAsync>Begin");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(Project.ProjectId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostAsync>End");
            return RedirectToPage("./Index");
        }

        private bool ProjectExists(string id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }

        public async Task<IActionResult> CreateProjectTask(string SectionId, DateOnly? startDate, DateOnly? endDate, string Name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<CreateProjectTask>Begin");
            try
            {
                var excecutingUser = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                var pt = new ProjectTask { CompanyId = excecutingUser.CompanyId,
                                           StartDate = startDate,
                                           EndDate = endDate, 
                                           ProjectTaskName = Name,
                                           ProjectSectionId = SectionId };
                pt = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Aufgabe angelegt", pt);
                _context.ProjectTask.Add(pt);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<CreateProjectTask>End");
            return Page();
        }
        public async Task<IActionResult> EditProjectTask(string taskId, DateOnly? startDate, DateOnly? endDate, string? name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<EditProjectTask>Begin");
            try
            {
                var pt = await _context.ProjectTask.FirstOrDefaultAsync(pt => pt.ProjectTaskId == taskId);
                if (startDate != null)
                {
                    pt.StartDate = startDate;
                }
                if (endDate != null)
                {
                    pt.EndDate = endDate;
                }
                if (name != null)
                {
                    pt.ProjectTaskName = name;
                }
                pt = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Aufgabe angelegt", pt);

                _context.ProjectTask.Update(pt);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<EditProjectTask>End");
            return Page();
        }

        public async Task<IActionResult> CreateProjectSection(string projectId, string Name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<CreateProjectSection>Begin");
            try
            {
                var excecutingUser = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                var section = new ProjectSection
                {
                    ProjectId = projectId,
                    ProjectSectionName = Name,
                    CompanyId = excecutingUser.CompanyId
                };
                section = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Abschnitt angelegt", section);
                _context.ProjectSection.Add(section);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<CreateProjectSection>End");
            return Page();
        }

        public async Task<IActionResult> EditProjectSection(string sectionId, string? Name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<EditProjectSection>Begin");
            try
            {
                var section = await _context.ProjectSection.FirstOrDefaultAsync(s => s.ProjectSectionId == sectionId);
                if (Name != null)
                {
                    section.ProjectSectionName = Name;
                }
                section = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Abschnitt bearbeitet", section);
                _context.ProjectSection.Update(section);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<EditProjectSection>End");
            return Page();
        }

        public async Task<IActionResult> CreateSubSection(string parentSectionId, string Name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<CreateSubSection>Begin");
            try
            {
                var excecutingUser = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                var sub = new ProjectSection
                {
                    ParentSectionId = parentSectionId,
                    ProjectSectionName = Name,
                    CompanyId = excecutingUser.CompanyId
                };
                sub = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Unterabschnitt angelegt", sub);
                _context.ProjectSection.Add(sub);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<CreateSubSection>End");
            return Page();
        }

        public async Task<IActionResult> EditSubSection(string subSectionId, string? Name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<EditSubSection>Begin");
            try
            {
                var sub = await _context.ProjectSection.FirstOrDefaultAsync(s => s.ProjectSectionId == subSectionId);
                if (Name != null)
                {
                    sub.ProjectSectionName = Name;
                }
                sub = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Unterabschnitt bearbeitet", sub);
                _context.ProjectSection.Update(sub);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<EditSubSection>End");
            return Page();
        }
    }
}
