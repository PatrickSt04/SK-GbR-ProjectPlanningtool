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
        public async Task<IActionResult> OnPostCreateProjectTaskAsync(string projectId, string SectionId, DateOnly? startDate, DateOnly? endDate, string Name, int? durationWorkDays)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateProjectTaskAsync>Begin");
            try
            {
                var executingUser = await new CustomUserManager(_context, _userManager)
                    .GetEmployeeAsync(_userManager.GetUserId(User));

                // Falls nur Dauer übergeben (Fallback serverseitig)
                if (startDate.HasValue && durationWorkDays.HasValue && durationWorkDays > 0 && !endDate.HasValue)
                {
                    endDate = await CalculateEndDateAsync(
                        startDate.Value, durationWorkDays.Value, executingUser.CompanyId);
                }

                var pt = new ProjectTask
                {
                    CompanyId = executingUser.CompanyId,
                    StartDate = startDate,
                    EndDate = endDate,
                    ProjectTaskName = Name,
                    ProjectSectionId = SectionId,
                };
                pt.State = await new StateManager(_context).getOpenState();
                pt = await _customObjectModifier.AddLatestModificationAsync(User, "Aufgabe angelegt", pt, true);

                _context.ProjectTask.Add(pt);
                await _context.SaveChangesAsync();

                TempData.SetMessage("Success", "Aufgabe erfolgreich angelegt.");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateProjectTaskAsync>End");

            return Origin == "Details"
                ? RedirectToPage("/Projects/Details", new { id = projectId })
                : RedirectToPage(new { id = projectId });
        }

        /// <summary>
        /// Berechnet das Enddatum ausgehend von einem Startdatum und einer Anzahl Arbeitstage,
        /// unter Berücksichtigung der DefaultWorkDays der Company und der Feiertage.
        /// </summary>
        private async Task<DateOnly?> CalculateEndDateAsync(
            DateOnly startDate, int workDays, string? companyId)
        {
            if (companyId == null || workDays < 1) return null;

            var company = await _context.Company
                .FirstOrDefaultAsync(c => c.CompanyId == companyId);

            if (company == null) return null;

            var allowedDays = company.DefaultWorkDays; // List<int>, ISO 1=Mo…7=So

            // Feiertage für diese Company laden
            var holidays = _context.HolidayCalendarEntry
                .Where(h => h.CompanyId == companyId && !h.DeleteFlag)
                .Select(h => h.HolidayDate)
                .ToHashSet();

            var current = startDate;
            int counted = 0;

            while (true)
            {
                int dow = current.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)current.DayOfWeek;
                if (allowedDays.Contains(dow) && !holidays.Contains(current))
                {
                    counted++;
                    if (counted >= workDays) break;
                }
                current = current.AddDays(1);
            }

            return current;
        }
        public async Task<IActionResult> OnPostCreateTaskCatalogTaskAsync(string projectId, string Name, DateOnly? startDate, DateOnly? endDate)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateProjectTaskAsync>Begin");
            try
            {
                var excecutingUser = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));


                var pt = new ProjectTaskCatalogTask
                {
                    CompanyId = excecutingUser.CompanyId,
                    TaskName = Name,
                    ProjectId = projectId,
                    StartDate = startDate,
                    EndDate = endDate,
                };
                pt.State = await new StateManager(_context).getOpenState();
                pt = await _customObjectModifier.AddLatestModificationAsync(User, "Aufgabe angelegt", pt, true);

                var ptFixCosts = new ProjectTaskFixCosts
                {
                    ProjectTaskCatalogTask = pt,
                    FixCosts = new List<ProjectTaskFixCosts.FixCost>(),
                };
                //Erst task zur DB hinzufügen
                _context.ProjectTaskCatalogTask.Add(pt);
                await _context.SaveChangesAsync();

                _context.ProjectTaskFixCosts.Add(ptFixCosts);
                await _context.SaveChangesAsync();

                //Dann fixcosts zum Task hinzuladen und updaten
                pt.ProjectTaskFixCosts = ptFixCosts;
                _context.ProjectTaskCatalogTask.Update(pt);
                await _context.SaveChangesAsync();

                //TaskCataLog Tagst zur Nav in Project hinzufügen
                var project = await _context.Project.FirstOrDefaultAsync(p => p.ProjectId == projectId);
                if (project == null)
                {
                    return NotFound();
                }
                if (project.ProjectTaskCatalogTasks == null)
                {
                    project.ProjectTaskCatalogTasks = new List<ProjectTaskCatalogTask>();
                }
                project.ProjectTaskCatalogTasks.Add(pt);

                _context.Project.Update(project);
                await _context.SaveChangesAsync();



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

        public async Task<IActionResult> OnPostEditProjectTaskAsync(string projectId, string taskId, DateOnly? startDate, DateOnly? endDate, string? name)
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
        public async Task<IActionResult> OnPostEditTaskCatalogTaskAsync(string projectId, string taskId, DateOnly? startDate, DateOnly? endDate, string? name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostEditProjectTaskAsync>Begin");
            try
            {
                var pt = await _context.ProjectTaskCatalogTask.FirstOrDefaultAsync(pt => pt.ProjectTaskCatalogTaskId == taskId);
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
                    pt.TaskName = name;
                }
                pt = await _customObjectModifier.AddLatestModificationAsync(User, "Aufgabe bearbeitet", pt, false);

                _context.ProjectTaskCatalogTask.Update(pt);
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

        public async Task<IActionResult> OnPostCreateMileStoneAsync(string sectionId, string Name, DateOnly startDate)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateMileStoneAsync>Begin");
            try
            {
                var excecutingUser = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                var milestone = new ProjectSectionMilestone
                {
                    CompanyId = excecutingUser.CompanyId,
                    MilestoneName = Name,
                    Date = startDate,
                    ProjectSectionId = sectionId
                };
                milestone = await _customObjectModifier.AddLatestModificationAsync(User, "Meilenstein angelegt", milestone, true);
                await _context.ProjectSectionMilestone.AddAsync(milestone);
                await _context.SaveChangesAsync();


                var section = await _context.ProjectSection.FirstOrDefaultAsync(ps => ps.ProjectSectionId == sectionId);
                if (section != null)
                {
                    var milestones = section.ProjectSectionMilestones;
                    if (milestones == null)
                    {
                        milestones = new List<ProjectSectionMilestone>();
                    }
                    milestones.Add(milestone);
                    section.ProjectSectionMilestones = milestones;
                    _context.ProjectSection.Update(section);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }

            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateMileStoneAsync>End");
            return RedirectToPage(new { id = Project.ProjectId });
        }

        public async Task<IActionResult> OnPostEditMileStoneAsync(string projectId, string milestoneId, string sectionId, string name, DateOnly startDate)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostEditMileStoneAsync>Begin");
            try
            {
                var milestone = await _context.ProjectSectionMilestone.FirstOrDefaultAsync(ms => ms.ProjectSectionMilestoneId == milestoneId);
                if (milestone == null)
                {
                    TempData.SetMessage("Danger", "Meilenstein nicht gefunden.");
                    return RedirectToPage(new { id = projectId });
                }

                milestone.ProjectSectionId = sectionId;
                milestone.MilestoneName = name;
                milestone.Date = startDate;
                milestone = await _customObjectModifier.AddLatestModificationAsync(User, "Meilenstein bearbeitet", milestone, false);

                _context.ProjectSectionMilestone.Update(milestone);
                await _context.SaveChangesAsync();
                TempData.SetMessage("Success", "Meilenstein erfolgreich geändert.");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }

            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostEditMileStoneAsync>End");
            return RedirectToPage(new { id = projectId });
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
                    "TaskCatalogTask" => await HandleDeleteTaskCatalogTask(wrapper.Data),
                    "ProjectSectionMilestone" => await HandleDeleteProjectSectionMilestone(wrapper.Data),
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

        private async Task<IActionResult> HandleDeleteProjectSectionMilestone(JsonElement data)
        {
            var milestoneId = data.GetProperty("itemId").GetString();
            if (string.IsNullOrWhiteSpace(milestoneId))
            {
                TempData.SetMessage("Danger", "Ungültiger Meilensteinbezeichner.");
                if (Origin != null && Origin == "Details")
                {
                    return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
                }
                else
                {
                    return RedirectToPage(new { id = Project.ProjectId });
                }
            }
            var milestone = await _context.ProjectSectionMilestone.FirstOrDefaultAsync(ms => ms.ProjectSectionMilestoneId == milestoneId);
            if (milestone == null)
            {
                TempData.SetMessage("Danger", "Meilenstein nicht gefunden.");
                if (Origin != null && Origin == "Details")
                {
                    return RedirectToPage("/Projects/Details", new { id = Project.ProjectId });
                }
                else
                {
                    return RedirectToPage(new { id = Project.ProjectId });
                }
            }
            _context.ProjectSectionMilestone.Remove(milestone);
            await _context.SaveChangesAsync();
            TempData.SetMessage("Success", "Meilenstein erfolgreich gelöscht.");
            return null;
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


        private async Task<IActionResult?> HandleDeleteTaskCatalogTask(JsonElement data)
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

            var task = await _context.ProjectTaskCatalogTask.FirstOrDefaultAsync(pt => pt.ProjectTaskCatalogTaskId == taskId);
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

            var tfc = await _context.ProjectTaskFixCosts.Where(hrg => hrg.TaskId == taskId).FirstOrDefaultAsync();

            if (tfc != null)
            {
                _context.ProjectTaskFixCosts.Remove(tfc);
                await _context.SaveChangesAsync();
            }

            _context.ProjectTaskCatalogTask.Remove(task);

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
