using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class ProjectStatisticsCalculator
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjectStatisticsCalculator(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Berechnet alle relevanten Statistik-Werte für ein Projekt.
        /// </summary>
        public async Task<ProjectStatistics> CalculateStatisticsAsync(string projectId, ClaimsPrincipal user)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                throw new ArgumentException("Projekt-ID darf nicht leer sein.", nameof(projectId));

            var employee = await GetEmployeeAsync(user);
            var sectionIds = await GetProjectSectionIdsAsync(projectId, employee.CompanyId);
            var completedStateId = await GetCompletedStateIdAsync();

            int totalTasks = await GetTaskCountAsync(sectionIds, employee.CompanyId);
            int completedTasks = await GetTaskCountAsync(sectionIds, employee.CompanyId, completedStateId);

            return new ProjectStatistics(totalTasks, completedTasks);
        }

        #region Private Helpers

        private async Task<Employee> GetEmployeeAsync(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            return await new CustomUserManager(_context, _userManager).GetEmployeeAsync(userId);
        }

        private async Task<List<string>> GetProjectSectionIdsAsync(string projectId, string companyId)
        {
            return await _context.ProjectSection
                .Where(ps => ps.ProjectId == projectId && ps.CompanyId == companyId)
                .Select(ps => ps.ProjectSectionId)
                .ToListAsync();
        }

        private async Task<string> GetCompletedStateIdAsync()
        {
            var state = await _context.State
                .FirstOrDefaultAsync(s => s.StateName == "Abgeschlossen")
                ?? throw new InvalidOperationException("Kein Status 'Abgeschlossen' gefunden.");

            return state.StateId;
        }

        private async Task<int> GetTaskCountAsync(
            List<string> sectionIds,
            string companyId,
            string? stateId = null)
        {
            var query = _context.ProjectTask
                .Where(pt => sectionIds.Contains(pt.ProjectSectionId))
                .Where(pt => pt.CompanyId == companyId)
                .Where(pt => pt.IsTaskCatalogEntry);

            if (!string.IsNullOrEmpty(stateId))
            {
                query = query.Where(pt => pt.StateId == stateId);
            }

            return await query.CountAsync();
        }

        #endregion
    }

    /// <summary>
    /// Dient als Rückgabemodell für berechnete Projektstatistiken.
    /// </summary>
    public record ProjectStatistics(int TotalTasks, int CompletedTasks)
    {
        public double Progress => TotalTasks == 0 ? 0 : (double)CompletedTasks / TotalTasks;
    }
}
