using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    // This file is used for editing the schedule of a project.
    public class EditModel : ProjectPageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public EditModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
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

                var project = await GetProjectAsync(id);
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
                    ProjectSectionId = SectionId
                };
                pt.State = await new StateManager(_context).getOpenState();
                pt = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Aufgabe angelegt", pt, true);
                _context.ProjectTask.Add(pt);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateProjectTaskAsync>End");
            return RedirectToPage(new { id = projectId });
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
                pt = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Aufgabe bearbeitet", pt, false);

                _context.ProjectTask.Update(pt);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostEditProjectTaskAsync>End");
            return RedirectToPage(new { id = projectId });
        }

        public async Task<IActionResult> OnPostCreateProjectSectionAsync(string projectId, string Name)
        {
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateProjectSectionAsync>Begin");
            try
            {
                var excecutingUser = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                var section = new ProjectSection
                {
                    ProjectId = projectId,
                    ProjectSectionName = Name,
                    CompanyId = excecutingUser.CompanyId
                };
                section = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Abschnitt angelegt", section, true);
                _context.ProjectSection.Add(section);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateProjectSectionAsync>End");
            return RedirectToPage(new { id = projectId });
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
                section = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Abschnitt bearbeitet", section, false);
                _context.ProjectSection.Update(section);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostEditProjectSectionAsync>End");
            return RedirectToPage(new { id = Project.ProjectId });
        }

        public async Task<IActionResult> OnPostCreateSubSectionAsync(string parentSectionId, string Name)
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
                    Project = parentSection.Project,
                    CompanyId = excecutingUser.CompanyId
                };
                sub = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Unterabschnitt angelegt", sub, true);
                _context.ProjectSection.Add(sub);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.Scheduling<OnPostCreateSubSectionAsync>End");
            return RedirectToPage(new { id = Project.ProjectId });
        }
        public async Task<IActionResult> OnPostDeleteSelectedAsync(string? objectJson)
        {
            try
            {
                await _logger.Log(null, User, objectJson, "Projects.Scheduling<OnDeleteSelectedAsync>Begin");

                if (string.IsNullOrWhiteSpace(objectJson))
                {
                    TempData.SetMessage("Danger", "Ungültige Anfrage: Kein JSON übergeben.");
                    return RedirectToPage(new { id = Project.ProjectId });
                }

                var wrapper = JsonSerializer.Deserialize<JsonWrapper>(objectJson);
                if (wrapper == null)
                {
                    TempData.SetMessage("Danger", "Fehler beim Deserialisieren des Objekts.");
                    return RedirectToPage(new { id = Project.ProjectId });
                }

                IActionResult? result = wrapper.Type switch
                {
                    "ProjectSection" => await HandleDeleteProjectSection(wrapper.Data),
                    "ProjectTask" => await HandleDeleteProjectTask(wrapper.Data),
                    _ => await HandleUnknownType(wrapper.Type)
                };

                await _logger.Log(null, User, objectJson, "Projects.Scheduling<OnDeleteSelectedAsync>End");

                return result ?? RedirectToPage(new { id = Project.ProjectId });
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
                    return RedirectToPage(new { id = Project.ProjectId });
                }

                var section = await _context.ProjectSection
                    .Include(ps => ps.SubSections)
                    .Include(ps => ps.ProjectTasks)
                    .FirstOrDefaultAsync(ps => ps.ProjectSectionId == sectionId);

                if (section == null)
                {
                    TempData.SetMessage("Danger", "Projektabschnitt nicht gefunden.");
                    return RedirectToPage(new { id = Project.ProjectId });
                }

                if ((section.SubSections?.Any() ?? false) || (section.ProjectTasks?.Any() ?? false))
                {
                    TempData.SetMessage("Warning", "Der Abschnitt enthält noch Unterabschnitte oder Aufgaben und kann daher nicht gelöscht werden.");
                    return RedirectToPage(new { id = Project.ProjectId });
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
            return RedirectToPage(new { id = Project.ProjectId });
        }
        private async Task<IActionResult?> HandleDeleteProjectTask(JsonElement data)
        {
            var taskId = data.GetProperty("itemId").GetString();
            if (string.IsNullOrWhiteSpace(taskId))
            {
                TempData.SetMessage("Danger", "Ungültiger Aufgabenbezeichner.");
                return RedirectToPage(new { id = Project.ProjectId });
            }

            var task = await _context.ProjectTask.FirstOrDefaultAsync(pt => pt.ProjectTaskId == taskId);
            if (task == null)
            {
                TempData.SetMessage("Danger", "Aufgabe nicht gefunden.");
                return RedirectToPage(new { id = Project.ProjectId });
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

