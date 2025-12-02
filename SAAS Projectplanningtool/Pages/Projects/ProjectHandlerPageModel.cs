using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using System.Text.Json;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class ProjectHandlerPageModel : ProjectPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        private readonly CustomObjectModifier _customObjectModifier;
        public ProjectHandlerPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
            _customObjectModifier = new CustomObjectModifier(_context, _userManager);
        }
        [BindProperty]
        public string? Origin { get; set; } = null;
        [BindProperty]
        public bool IsTaskCatalogEntry { get; set; } = true;
        [BindProperty]
        public bool IsScheduleEntry { get; set; } = true;
        public async Task<IActionResult> OnPostCreateProjectTaskAsync(string projectId, string SectionId, DateOnly? startDate, DateOnly? endDate, string Name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateProjectTaskAsync>Begin");
            try
            {
                var excecutingUser = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));


                var pt = new ProjectTask
                {
                    CompanyId = excecutingUser.CompanyId,
                    StartDate = startDate,
                    EndDate = endDate,
                    ProjectTaskName = Name,
                    ProjectSectionId = SectionId,
                    IsTaskCatalogEntry = IsTaskCatalogEntry,
                    IsScheduleEntry = IsScheduleEntry
                };
                pt.State = await new StateManager(_context).getOpenState();
                pt = await _customObjectModifier.AddLatestModificationAsync(User, "Aufgabe angelegt", pt, true);

                var ptFixCosts = new ProjectTaskFixCosts
                {
                    ProjectTask = pt,
                    FixCosts = new List<ProjectTaskFixCosts.FixCost>(),
                    ProjectTaskId = pt.ProjectTaskId
                };
                //Erst task zur DB hinzufügen
                _context.ProjectTask.Add(pt);
                await _context.SaveChangesAsync();

                if (IsTaskCatalogEntry) { 
                //Dann fixcosts zur DB hinzufügen
                _context.ProjectTaskFixCosts.Add(ptFixCosts);
                await _context.SaveChangesAsync();

                //Dann fixcosts zum Task hinzuladen und updaten
                pt.ProjectTaskFixCosts = ptFixCosts;
                _context.ProjectTask.Update(pt);
                await _context.SaveChangesAsync();

                }




                TempData.SetMessage("Success", "Aufgabe erfolgreich angelegt.");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateProjectTaskAsync>End");
            if (Origin != null && Origin == "Details")
            {
                return RedirectToPage("/Projects/Details", new { id = projectId });
            }
            else
            {
                return RedirectToPage(new { id = projectId });
            }
        }

        public async Task<IActionResult> OnPostEditProjectTaskAsync(string projectId, string taskId, DateOnly? startDate, DateOnly? endDate, string? name, bool isTaskCatalogEntry, bool isScheduleEntry)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostEditProjectTaskAsync>Begin");
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
                pt.IsTaskCatalogEntry = isTaskCatalogEntry;
                pt.IsScheduleEntry = isScheduleEntry;
                pt = await _customObjectModifier.AddLatestModificationAsync(User, "Aufgabe bearbeitet", pt, false);

                _context.ProjectTask.Update(pt);
                await _context.SaveChangesAsync();
                TempData.SetMessage("Success", "Aufgabe erfolgreich geändert.");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostEditProjectTaskAsync>End");
            if (Origin != null && Origin == "Details")
            {
                return RedirectToPage("/Projects/Details", new { id = projectId });
            }
            else
            {
                return RedirectToPage(new { id = projectId });
            }
        }

        public async Task<IActionResult> OnPostCreateProjectSectionAsync(string projectId, string? parentSectionId, string Name)
        {
            try
            {
                if (parentSectionId != null)
                {
                    await OnPostCreateSubSectionAsync(parentSectionId, Name);
                }
                else
                {
                    await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateProjectSectionAsync>Begin");


                    var excecutingUser = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                    var section = new ProjectSection
                    {
                        ProjectId = projectId,
                        ProjectSectionName = Name,
                        CompanyId = excecutingUser.CompanyId
                    };
                    section = await _customObjectModifier.AddLatestModificationAsync(User, "Abschnitt angelegt", section, true);
                    _context.ProjectSection.Add(section);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateProjectSectionAsync>End");
            TempData.SetMessage("Success", "Abschnitt erfolgreich angelegt.");
            if (Origin != null && Origin == "Details")
            {
                return RedirectToPage("/Projects/Details", new { id = projectId });
            }
            else
            {
                return RedirectToPage(new { id = projectId });
            }

        }

        public async Task<IActionResult> OnPostEditProjectSectionAsync(string sectionId, string? Name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostEditProjectSectionAsync>Begin");
            try
            {
                var section = await _context.ProjectSection.FirstOrDefaultAsync(s => s.ProjectSectionId == sectionId);
                if (Name != null)
                {
                    section.ProjectSectionName = Name;
                }
                section = await _customObjectModifier.AddLatestModificationAsync(User, "Abschnitt bearbeitet", section, false);
                _context.ProjectSection.Update(section);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostEditProjectSectionAsync>End");
            TempData.SetMessage("Success", "Abschnitt erfolgreich geändert.");
            if (Origin != null && Origin == "Details")
            {
                return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
            }
            else
            {
                return RedirectToPage(new { id = Project.ProjectId });
            }
        }

        private async Task OnPostCreateSubSectionAsync(string parentSectionId, string Name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateSubSectionAsync>Begin");
            try
            {
                var excecutingUser = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                var parentSection = await _context.ProjectSection.Include(s => s.Project).FirstOrDefaultAsync(s => s.ProjectSectionId == parentSectionId);
                var sub = new ProjectSection
                {
                    ParentSectionId = parentSectionId,
                    ProjectSectionName = Name,
                    ProjectId = parentSection.Project.ProjectId,
                    CompanyId = excecutingUser.CompanyId
                };
                sub = await _customObjectModifier.AddLatestModificationAsync(User, "Unterabschnitt angelegt", sub, true);
                await _context.ProjectSection.AddAsync(sub);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateSubSectionAsync>End");
        }
        public async Task<IActionResult> OnPostDeleteSelectedAsync(string? objectJson)
        {
            try
            {
                await _logger.Log(null, User, objectJson, "Projects.Scheduling<OnDeleteSelectedAsync>Begin");

                if (string.IsNullOrWhiteSpace(objectJson))
                {
                    TempData.SetMessage("Danger", "Ungültige Anfrage: Kein JSON übergeben.");
                    if (Origin != null && Origin == "Details")
                    {
                        return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
                    }
                    else
                    {
                        return RedirectToPage(new { id = Project.ProjectId });
                    }
                }

                var wrapper = JsonSerializer.Deserialize<JsonWrapper>(objectJson);
                if (wrapper == null)
                {
                    TempData.SetMessage("Danger", "Fehler beim Deserialisieren des Objekts.");
                    if (Origin != null && Origin == "Details")
                    {
                        return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
                    }
                    else
                    {
                        return RedirectToPage(new { id = Project.ProjectId });
                    }
                }

                IActionResult? result = wrapper.Type switch
                {
                    "ProjectSection" => await HandleDeleteProjectSection(wrapper.Data),
                    "ProjectTask" => await HandleDeleteProjectTask(wrapper.Data),
                    _ => await HandleUnknownType(wrapper.Type)
                };

                await _logger.Log(null, User, objectJson, "Projects.Scheduling<OnDeleteSelectedAsync>End");

                if (result != null)
                {
                    return result;
                }

                if (Origin != null && Origin == "Details")
                {
                    return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
                }
                else
                {
                    return RedirectToPage(new { id = Project.ProjectId });
                }
            }
            catch (Exception ex)
            {
                var errorLogId = _logger.Log(ex, User, objectJson, "ERROR: Projects.Scheduling<OnPostDeleteSelectedAsync>");
                return RedirectToPage("/Error", new { id = errorLogId });
            }
        }


        private async Task<IActionResult?> HandleDeleteProjectSection(JsonElement data)
        {
            try
            {
                var sectionId = data.GetProperty("itemId").GetString();
                if (string.IsNullOrWhiteSpace(sectionId))
                {
                    TempData.SetMessage("Danger", "Ungültiger Abschnittsbezeichner.");
                    if (Origin != null && Origin == "Details")
                    {
                        return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
                    }
                    else
                    {
                        return RedirectToPage(new { id = Project.ProjectId });
                    }
                }

                var section = await _context.ProjectSection
                    .Include(ps => ps.SubSections)
                    .Include(ps => ps.ProjectTasks)
                    .FirstOrDefaultAsync(ps => ps.ProjectSectionId == sectionId);

                if (section == null)
                {
                    TempData.SetMessage("Danger", "Projektabschnitt nicht gefunden.");
                    if (Origin != null && Origin == "Details")
                    {
                        return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
                    }
                    else
                    {
                        return RedirectToPage(new { id = Project.ProjectId });
                    }
                }

                if ((section.SubSections?.Any() ?? false) || (section.ProjectTasks?.Any() ?? false))
                {
                    TempData.SetMessage("Warning", "Der Abschnitt enthält noch Unterabschnitte oder Aufgaben und kann daher nicht gelöscht werden.");
                    if (Origin != null && Origin == "Details")
                    {
                        return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
                    }
                    else
                    {
                        return RedirectToPage(new { id = Project.ProjectId });
                    }
                }

                _context.ProjectSection.Remove(section);
                await _context.SaveChangesAsync();
                TempData.SetMessage("Success", "Abschnitt erfolgreich gelöscht.");
                return null;
            }
            catch (Exception)
            {
                throw; // harter Fehler -> ErrorPage
            }
        }
        private async Task<IActionResult> HandleUnknownType(string? type)
        {
            TempData.SetMessage("Danger", $"Unbekannter Objekttyp: {type}");
            if (Origin != null && Origin == "Details")
            {
                return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
            }
            else
            {
                return RedirectToPage(new { id = Project.ProjectId });
            }
        }
        private async Task<IActionResult?> HandleDeleteProjectTask(JsonElement data)
        {
            var taskId = data.GetProperty("itemId").GetString();
            if (string.IsNullOrWhiteSpace(taskId))
            {
                TempData.SetMessage("Danger", "Ungültiger Aufgabenbezeichner.");
                if (Origin != null && Origin == "Details")
                {
                    return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
                }
                else
                {
                    return RedirectToPage(new { id = Project.ProjectId });
                }
            }

            var task = await _context.ProjectTask.FirstOrDefaultAsync(pt => pt.ProjectTaskId == taskId);
            if (task == null)
            {
                TempData.SetMessage("Danger", "Aufgabe nicht gefunden.");
                if (Origin != null && Origin == "Details")
                {
                    return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
                }
                else
                {
                    return RedirectToPage(new { id = Project.ProjectId });
                }
            }
            //Selected HRGS for Task delete
            var hrgs = await _context.ProjectTaskHourlyRateGroup.Where(hrg => hrg.ProjectTaskId == taskId).ToListAsync();
            if (hrgs.Any())
            {
                _context.ProjectTaskHourlyRateGroup.RemoveRange(hrgs);
            }


            _context.ProjectTask.Remove(task);
            await _context.SaveChangesAsync();
            TempData.SetMessage("Success", "Aufgabe erfolgreich gelöscht.");
            return null;
        }
    }
    public class TempDataMessage
    {
        public string Type { get; set; } = "Error"; // oder "Success", "Info"
        public string Message { get; set; } = string.Empty;
    }
    public static class TempDataExtensions
    {
        public static void SetMessage(this ITempDataDictionary tempData, string type, string message)
        {
            var temp = JsonSerializer.Serialize(new TempDataMessage { Type = type, Message = message });
            tempData["Message"] = temp;
        }

        public static TempDataMessage? GetMessage(this ITempDataDictionary tempData)
        {
            if (tempData.TryGetValue("Message", out var obj) && obj is string json)
            {
                return JsonSerializer.Deserialize<TempDataMessage>(json);
            }

            return null;
        }
    }

    public class JsonWrapper
    {
        public string Type { get; set; } = "";
        public JsonElement Data { get; set; }
    }
}

