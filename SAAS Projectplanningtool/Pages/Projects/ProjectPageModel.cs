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

        [BindProperty]
        public Project Project { get; set; } = default!;

        public SelectList? Customers { get; set; }
        public SelectList? ProjectLeads { get; set; }


        [BindProperty(SupportsGet = true)]
        //Feiertage für das Erstellen von ProjectTask über Startdatum und Arbeitstage
        public List<string> HolidayDates { get; set; } = new();


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
        protected async Task<Employee?> GetEmployeeAsync()
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

        public async Task SetHolidaysBindingAsync(string companyId)
        {
            HolidayDates = await _context.HolidayCalendarEntry
                .Where(h => h.CompanyId == companyId && !h.DeleteFlag)
                .Select(h => h.HolidayDate.ToString("yyyy-MM-dd"))   // DateOnly → string
                .ToListAsync();
        }

        public async Task SetProjectBindingAsync(string projectId)
        {
            Project = await GetProjectAsync(projectId);
        }
        //private async Task<Project?> GetProjectAsync(string projectId)
        //{

        //    var Project = await _context.Project
        //        //.AsNoTracking()

        //        // --- Simple Includes ---
        //        .Include(p => p.LatestModifier)
        //        .Include(p => p.Company)
        //        .Include(p => p.CreatedByEmployee)
        //        .Include(p => p.Customer)
        //        .Include(p => p.ResponsiblePerson)
        //        .Include(p => p.State)

        //        // --- Budget & Recalculations ---
        //        .Include(p => p.ProjectBudget)
        //            .ThenInclude(pb => pb.BudgetRecalculations)
        //                .ThenInclude(br => br.RecalculatedBy)

        //        // --- ProjectSections ---
        //        .Include(p => p.ProjectSections)
        //            // Tasks der Sections (nur Schedule-Tasks)
        //            .ThenInclude(ps => ps.ProjectTasks)
        //                .ThenInclude(pt => pt.State)
        //        .Include(p => p.ProjectSections)
        //            .ThenInclude(ps => ps.ProjectTasks)
        //                .ThenInclude(pt => pt.ProjectTaskHourlyRateGroups)
        //                    .ThenInclude(h => h.HourlyRateGroup)

        //        .Include(p => p.ProjectTaskCatalogTasks)
        //            .ThenInclude(ptc => ptc.State)
        //        .Include(p => p.ProjectTaskCatalogTasks)
        //        .ThenInclude(ptc => ptc.ProjectTaskFixCosts)
        //        .ThenInclude(ptfc => ptfc.FixCosts)
        //        .FirstOrDefaultAsync(p => p.ProjectId == projectId);



        //    if (Project.ProjectSections != null)
        //    {
        //        foreach (var section in Project.ProjectSections)
        //        {
        //            // Milestones nachträglich laden -> andernfalls imperformant
        //            section.ProjectSectionMilestones = await GetProjectSectionMilestones(section);

        //            //TODO: Für Alle PS müssen die Subsections nachträglich inkludiert werden --> Im EF Include kann man nicht beliebig tief verschachteln
        //            await LoadProjectSectionSubSections(section);


        //            section.State = await new StateManager(_context).GetSectionState(section.ProjectSectionId);
        //            if (section.SubSections != null)
        //            {
        //                foreach (var subsection in section.SubSections)
        //                {
        //                    subsection.ProjectSectionMilestones = await GetProjectSectionMilestones(subsection);
        //                    subsection.State = await new StateManager(_context).GetSectionState(subsection.ProjectSectionId);
        //                }
        //            }
        //        }
        //    }
        //    return Project;
        //}

        //private async Task<List<ProjectSectionMilestone>> GetProjectSectionMilestones(ProjectSection section)
        //{
        //    var milestones = _context.ProjectSectionMilestone
        //        .Where(m => m.ProjectSectionId == section.ProjectSectionId)
        //        .Include(m => m.LatestModifier)
        //        .Include(m => m.CreatedByEmployee)
        //        .ToList();
        //    return milestones;
        //}

        //private async Task LoadProjectSectionSubSections(ProjectSection section)
        //{
        //    // Lade die direkten SubSections
        //    await _context.Entry(section)
        //        .Collection(s => s.SubSections)
        //        .Query()
        //        .Include(ss => ss.State)
        //        .Include(ss => ss.ProjectTasks)
        //            .ThenInclude(pt => pt.State)
        //        .Include(ss => ss.ProjectTasks)
        //            .ThenInclude(pt => pt.ProjectTaskHourlyRateGroups)
        //                .ThenInclude(h => h.HourlyRateGroup)
        //        .LoadAsync();

        //    // Rekursiv für jede SubSection wiederholen
        //    if (section.SubSections != null && section.SubSections.Any())
        //    {
        //        foreach (var subSection in section.SubSections)
        //        {
        //            LoadProjectSectionSubSections(subSection);
        //        }
        //    }
        //}
        private async Task<Project?> GetProjectAsync(string projectId)
        {
            var project = await _context.Project
                .Include(p => p.LatestModifier)
                .Include(p => p.Company)
                .Include(p => p.CreatedByEmployee)
                .Include(p => p.Customer)
                .Include(p => p.ResponsiblePerson)
                .Include(p => p.State)
                .Include(p => p.ProjectBudget)
                    .ThenInclude(pb => pb.BudgetRecalculations)
                        .ThenInclude(br => br.RecalculatedBy)
                .Include(p => p.ProjectSections)
                    .ThenInclude(ps => ps.ProjectTasks)
                        .ThenInclude(pt => pt.State)
                .Include(p => p.ProjectSections)
                    .ThenInclude(ps => ps.ProjectTasks)
                        .ThenInclude(pt => pt.ProjectTaskHourlyRateGroups)
                            .ThenInclude(h => h.HourlyRateGroup)
                .Include(p => p.ProjectTaskCatalogTasks)
                    .ThenInclude(ptc => ptc.State)
                .Include(p => p.ProjectTaskCatalogTasks)
                    .ThenInclude(ptc => ptc.ProjectTaskFixCosts)
                        .ThenInclude(ptfc => ptfc.FixCosts)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project?.ProjectSections == null || !project.ProjectSections.Any())
                return project;

            // Sammle alle SectionIds (inklusive aller SubSections)
            var allSectionIds = await GetAllSectionIdsRecursive(project.ProjectSections.Select(s => s.ProjectSectionId).ToList());

            // Lade alle Milestones in einer Abfrage
            var allMilestones = await _context.ProjectSectionMilestone
                .Where(m => allSectionIds.Contains(m.ProjectSectionId))
                .Include(m => m.LatestModifier)
                .Include(m => m.CreatedByEmployee)
                .ToListAsync();

            var milestonesBySection = allMilestones
                .GroupBy(m => m.ProjectSectionId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Lade alle States in einer Abfrage
            var stateManager = new StateManager(_context);
            var allStates = await GetAllSectionStates(allSectionIds, stateManager);

            // Lade alle SubSections rekursiv
            await LoadAllSubSectionsRecursive(project.ProjectSections.ToList());

            // Weise Milestones und States zu
            AssignMilestonesAndStates(project.ProjectSections, milestonesBySection, allStates);

            return project;
        }

        private async Task<List<string>> GetAllSectionIdsRecursive(List<string> sectionIds)
        {
            var allIds = new List<string>(sectionIds);
            var currentLevelIds = sectionIds;

            while (currentLevelIds.Any())
            {
                var subSectionIds = await _context.ProjectSection
                    .Where(ps => currentLevelIds.Contains(ps.ParentSectionId))
                    .Select(ps => ps.ProjectSectionId)
                    .ToListAsync();

                if (!subSectionIds.Any())
                    break;

                allIds.AddRange(subSectionIds);
                currentLevelIds = subSectionIds;
            }

            return allIds;
        }

        private async Task<Dictionary<string, SAAS_Projectplanningtool.Models.IndependentTables.State>> GetAllSectionStates(
            List<string> sectionIds, StateManager stateManager)
        {
            var states = new Dictionary<string, SAAS_Projectplanningtool.Models.IndependentTables.State>();

            foreach (var sectionId in sectionIds)
            {
                var state = await stateManager.GetSectionState(sectionId);
                if (state != null)
                {
                    states[sectionId] = state;
                }
            }

            return states;
        }

        private async Task LoadAllSubSectionsRecursive(List<ProjectSection> sections)
        {
            if (!sections.Any())
                return;

            foreach (var section in sections)
            {
                await _context.Entry(section)
                    .Collection(s => s.SubSections)
                    .Query()
                    .Include(ss => ss.State)
                    .Include(ss => ss.ProjectTasks)
                        .ThenInclude(pt => pt.State)
                    .Include(ss => ss.ProjectTasks)
                        .ThenInclude(pt => pt.ProjectTaskHourlyRateGroups)
                            .ThenInclude(h => h.HourlyRateGroup)
                    .LoadAsync();
            }

            // Sammle alle SubSections für die nächste Ebene
            var allSubSections = sections
                .Where(s => s.SubSections != null && s.SubSections.Any())
                .SelectMany(s => s.SubSections)
                .ToList();

            if (allSubSections.Any())
            {
                await LoadAllSubSectionsRecursive(allSubSections);
            }
        }

        private void AssignMilestonesAndStates(
            IEnumerable<ProjectSection> sections,
            Dictionary<string, List<ProjectSectionMilestone>> milestonesBySection,
            Dictionary<string, SAAS_Projectplanningtool.Models.IndependentTables.State> statesBySection)
        {
            foreach (var section in sections)
            {
                if (milestonesBySection.TryGetValue(section.ProjectSectionId, out var milestones))
                {
                    section.ProjectSectionMilestones = milestones;
                }

                if (statesBySection.TryGetValue(section.ProjectSectionId, out var state))
                {
                    section.State = state;
                }

                if (section.SubSections != null && section.SubSections.Any())
                {
                    AssignMilestonesAndStates(section.SubSections, milestonesBySection, statesBySection);
                }
            }
        }
    }
}
