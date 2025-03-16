using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.IndependentTables;

namespace SAAS_Projectplanningtool.CustomManagers
{
    public class StateManager
    {
        private readonly ApplicationDbContext _context;

        public StateManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<State> GetProjectState(string projectId)
        {
            if (projectId != null)
            {
                var totaltasks = await _context.ProjectTask
                    .Include(ps => ps.ProjectSection)
                    .Where(task => task.ProjectSection.ProjectId == projectId)
                    .CountAsync();

                var delayedTasks = await _context.ProjectTask
                    .Include(ps => ps.ProjectSection)
                    .Where(task => task.ProjectSection.ProjectId == projectId)
                    .Include(s => s.State)
                    .Where(task => task.State.StateName == "In Verzug")
                    .CountAsync();

                if (delayedTasks != 0)
                {
                    return await getState("In Verzug");
                }
                else
                {
                    var openTasks = await _context.ProjectTask
                        .Include(ps => ps.ProjectSection)
                        .Where(task => task.ProjectSection.ProjectId == projectId)
                        .Include(s => s.State)
                        .Where(task => task.State.StateName == "Offen")
                        .CountAsync();

                    if (openTasks == totaltasks)
                    {
                        return await getState("Offen");
                    }
                    else
                    {
                        var closedTasks = await _context.ProjectTask
                            .Include(ps => ps.ProjectSection)
                            .Where(task => task.ProjectSection.ProjectId == projectId)
                            .Include(s => s.State)
                            .Where(task => task.State.StateName == "Abgeschlossen")
                            .CountAsync();

                        if (closedTasks == totaltasks)
                        {
                            return await getState("Abgeschlossen");
                        }
                        else
                        {
                            return await getState("In Bearbeitung");
                        }
                    }
                }
            }
            else
            {
                return await getState("Offen");
            }
        }

        public async Task<State> GetSectionState(string sectionId)
        {
            if (sectionId != null)
            {
                async Task<List<string>> GetAllSubSectionIdsAsync(string sectionId)
                {
                    var subSectionIds = await _context.ProjectSection
                        .Where(section => section.ParentSectionId == sectionId)
                        .Select(section => section.ProjectSectionId)
                        .ToListAsync();

                    var nestedSubSections = new List<string>();
                    foreach (var subId in subSectionIds)
                    {
                        nestedSubSections.AddRange(await GetAllSubSectionIdsAsync(subId));
                    }

                    return subSectionIds.Concat(nestedSubSections).ToList();
                }

                var allSectionIds = await GetAllSubSectionIdsAsync(sectionId);
                allSectionIds.Add(sectionId);

                var totaltasks = await _context.ProjectTask
                    .Where(task => allSectionIds.Contains(task.ProjectSectionId))
                    .CountAsync();

                var delayedTasks = await _context.ProjectTask
                    .Where(task => allSectionIds.Contains(task.ProjectSectionId))
                    .Include(s => s.State)
                    .Where(task => task.State.StateName == "In Verzug")
                    .CountAsync();

                if (delayedTasks != 0)
                {
                    return await getState("In Verzug");
                }
                else
                {
                    var openTasks = await _context.ProjectTask
                        .Where(task => allSectionIds.Contains(task.ProjectSectionId))
                        .Include(s => s.State)
                        .Where(task => task.State.StateName == "Offen")
                        .CountAsync();

                    if (openTasks == totaltasks)
                    {
                        return await getState("Offen");
                    }
                    else
                    {
                        var closedTasks = await _context.ProjectTask
                            .Where(task => allSectionIds.Contains(task.ProjectSectionId))
                            .Include(s => s.State)
                            .Where(task => task.State.StateName == "Abgeschlossen")
                            .CountAsync();

                        if (closedTasks == totaltasks)
                        {
                            return await getState("Abgeschlossen");
                        }
                        else
                        {
                            return await getState("In Bearbeitung");
                        }
                    }
                }
            }
            else
            {
                return await getState("Offen");
            }
        }

        public async Task<State> getState(string name)
        {
            return await _context.State
                .Where(s => s.StateName.Equals(name))
                .FirstOrDefaultAsync();
        }
    }
}

