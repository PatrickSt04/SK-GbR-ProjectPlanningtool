using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using Microsoft.AspNetCore.Mvc;
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class ProjectPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        //Diese Klasse wird in den Razor Pages verwendet, um die SelectLists für Kunden und Projektleiter zu erstellen.
        //Zudem kapselt sie Methoden, um Redundanz zu vermeiden (Beispiel: Archivieren und Ent-Archivieren).
        public ProjectPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public SelectList? Customers { get; set; }
        public SelectList? ProjectLeads { get; set; }


        public async Task<IActionResult> OnPostSetProjectArchivedAsync(string id)
        {
            await _logger.Log(null, User, null, "Projects/Details<SetProjectArchived>Beginn");
            try
            {
                var project = await _context.Project
                    .Include(p => p.Company)
                    .Include(p => p.Customer)
                    .Include(p => p.LatestModifier)
                    .Include(p => p.ProjectBudget)
                    .Include(p => p.State)
                    .FirstOrDefaultAsync(m => m.ProjectId == id);
                if (project == null)
                {
                    return NotFound();
                }
                project.IsArchived = true;
                project = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Projekt wurde archiviert", project, false);

                _context.Update(project);
                await _context.SaveChangesAsync();

                await _logger.Log(null, User, null, "Projects/Details<SetProjectArchived>End");
                // Nachdem das Projekt archiviert wurde soll der User wieder auf die Start page geleitet werden
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, "Projects/Details<SetProjectArchived>End") });
            }
        }
        public async Task<IActionResult> OnPostUndoProjectArchivedAsync(string id, string origin)
        {
            await _logger.Log(null, User, null, "Projects/Details<UndoProjectArchived>Beginn");
            try
            {
                var project = await _context.Project
                    .Include(p => p.Company)
                    .Include(p => p.Customer)
                    .Include(p => p.LatestModifier)
                    .Include(p => p.ProjectBudget)
                    .Include(p => p.State)
                    .FirstOrDefaultAsync(m => m.ProjectId == id);

                if (project == null)
                {
                    return NotFound();
                }

                project.IsArchived = false;
                project = await new CustomObjectModifier(_context, _userManager)
                    .AddLatestModificationAsync(User, "Projekt wurde wieder hergestellt", project, false);

                _context.Update(project);
                await _context.SaveChangesAsync();

                await _logger.Log(null, User, null, "Projects/Details<UndoProjectArchived>End");

                // Dynamisches Redirect je nach Ursprung
                return origin switch
                {
                    "Archived" => RedirectToPage("/Projects/Archive"),
                    "Details" => RedirectToPage("/Index"),
                    _ => RedirectToPage("/Index")
                };
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new
                {
                    id = await _logger.Log(ex, User, null, "Projects/Details<UndoProjectArchived>End")
                });
            }
        }



        // Lädt die SelectList mit allen Kunden (z.B. für Dropdown)
        public async Task PublishCustomersAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/ProjectPageModel<PublishCustomers>Beginn");

                var employee = await GetEmployeeAsync();
                var customers = await _context.Customer
                    .Where(c => c.CompanyId == employee.CompanyId) // Hier wird die CompanyId gefiltert
                    .Where(c => c.DeleteFlag == false)
                    .OrderBy(c => c.CustomerName)
                    .ToListAsync();

                Customers = new SelectList(customers, nameof(Customer.CustomerId), nameof(Customer.CustomerName));
                await _logger.Log(null, User, null, "Projects/ProjectPageModel<PublishCustomers>End");
            }
            catch (Exception ex)
            {
                await _logger.Log(ex, User, Customers, "EXCEPTION: Projects/ProjectPageModel<PublishCustomers>End");
                Customers = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            }
        }

        // Lädt die SelectList mit allen Benutzern mit der Rolle "Project Manager"
        public async Task PublishProjectLeadsAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/ProjectPageModel<PublishProjectLeadsAsync>Beginn");
                var plannerRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "Planner");

                if (plannerRole == null)
                {
                    // Rolle "Planner" existiert nicht
                    ProjectLeads = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
                    await _logger.Log(null, User, null, "ERROR: NO EMPLOYEE FOUND: Projects/ProjectPageModel<PublishProjectLeadsAsync>");
                    return;
                }
                var employee = await GetEmployeeAsync();

                var projectLeads = _context.Employee
                    .Where(e => e.IdentityRoleId == plannerRole.Id) // Hier wird die Rolle "Planner" gefiltert
                    .Where(e => e.CompanyId == employee.CompanyId) // Hier wird die CompanyId gefiltert
                    .Where(e => e.DeleteFlag == false)
                    .Select(u => new
                    {
                        Id = u.EmployeeId,
                        Name = u.EmployeeDisplayName
                    })
                    .OrderBy(u => u.Name)
                    .ToList();

                ProjectLeads = new SelectList(projectLeads, "Id", "Name");
                await _logger.Log(null, User, null, "Projects/ProjectPageModel<PublishProjectLeadsAsync>End");
            }
            catch (Exception ex)
            {
                await _logger.Log(ex, User, ProjectLeads, "EXCEPTION: Projects/ProjectPageModel<PublishProjectLeadsAsync>End");
                ProjectLeads = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            }
        }
        private async Task<Employee?> GetEmployeeAsync()
        {
            try
            {
                var employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                return employee;
            }
            catch (Exception ex)
            {
                await _logger.Log(ex, User, null, "EXCEPTION: Projects/ProjectPageModel<GetEmployeeAsync>");
                return null;
            }
        }
        public async Task<Project?> GetProjectAsync(string projectId)
        {
            var Project = await _context.Project
                .Include(p => p.LatestModifier)
                .Include(p => p.Company)
                .Include(p => p.CreatedByEmployee)
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
                .FirstOrDefaultAsync(m => m.ProjectId == projectId);
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
            return Project;
        }
    }
}
