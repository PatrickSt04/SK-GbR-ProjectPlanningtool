using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectAuthManager(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        ClaimsPrincipal user,
        CustomUserManager customUserManager)
        : AuthorizationManager(userManager, roleManager, context, user)
    {
        public ProjectCreateAuthManager ProjectCreateAuthManager { get; } = new(userManager, roleManager, context, user);
        public ProjectIndexAuthManager ProjectIndexAuthManager { get; } = new(userManager, roleManager, context, user);
        public ProjectDetailsAuthManager ProjectDetailsAuthManager { get; } = new(userManager, roleManager, context, user);
        public ProjectTaskCatalogAuthManager ProjectTaskCatalogAuthManager { get; } = new(userManager, roleManager, context, user);
        public ProjectStructureAuthManager ProjectStructureAuthManager { get; } = new(userManager, roleManager, context, user);
        public ProjectBudgetAuthManager ProjectBudgetAuthManager { get; } = new(userManager, roleManager, context, user);
        public ProjectArchiveAuthManager ProjectArchiveAuthManager { get; } = new(userManager, roleManager, context, user);

        /// <summary>
        /// Prüft, ob der aktuelle Benutzer Zugriff auf ein bestimmtes Projekt hat.
        /// - Planner/Admin: Alle Projekt-IDs der Company
        /// - Viewer: Nur freigegebene Projekt-IDs
        /// </summary>
        public virtual async Task<bool> CanViewProject(string projectId)
        {

            if (!await IsViewerLicense())
                return true;
            // Viewer darf nur Projekte sehen, die für ihn freigegeben sind
            var employee = await GetCurrentEmployeeAsync();

            return employee is not null
                && await _context.ProjectEmployeeViewerShare
                    .AsNoTracking()
                    .AnyAsync(share => share.ProjectId == projectId && share.EmployeeId == employee.EmployeeId);
        }
        /// <summary>
        /// Ruft alle für den aktuellen Benutzer zugreifbaren Projekt-IDs ab.
        /// - Planner/Admin: Alle Projekt-IDs der Company
        /// - Viewer: Nur freigegebene Projekt-IDs
        /// </summary>
        public virtual async Task<IEnumerable<string>> GetAccessibleProjectIds(string companyId)
        {
            // Planner und Admin sehen alle Projekte
            if (await IsPlannerLicense() || await IsAdminLicense())
            {
                return await _context.Project
                    .AsNoTracking()
                    .Where(p => p.CompanyId == companyId)
                    .Select(p => p.ProjectId)
                    .ToListAsync();
            }

            // Viewer sieht nur freigegebene Projekte
            if (await IsViewerLicense())
            {
                return await GetViewerAccessibleProjectIds(companyId);
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Ruft alle für einen Viewer freigegebenen Projekt-IDs ab.
        /// </summary>
        private async Task<IEnumerable<string>> GetViewerAccessibleProjectIds(string companyId)
        {
            var employee = await GetCurrentEmployeeAsync();
            if (employee is null)
                return Enumerable.Empty<string>();

            return await _context.ProjectEmployeeViewerShare
                .AsNoTracking()
                .Where(share => share.CompanyId == companyId && share.EmployeeId == employee.EmployeeId)
                .Select(share => share.ProjectId)
                .ToListAsync();
        }
        private async Task<Employee?> GetCurrentEmployeeAsync()
        {
            var userId = _userManager.GetUserId(_user);
            if (userId is null)
                return null;
            return await customUserManager.GetEmployeeAsync(userId);
        }
    }
}