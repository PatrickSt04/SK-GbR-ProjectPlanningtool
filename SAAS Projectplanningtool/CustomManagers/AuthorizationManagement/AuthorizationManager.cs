using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class AuthorizationManager
    {
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly ApplicationDbContext _context;
        protected readonly ClaimsPrincipal _user;

        public AuthorizationManager(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ClaimsPrincipal user)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _user = user;
        }

        private async Task<IdentityUser?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(_user);
        }

        private async Task<IList<string>> GetUserRolesAsync(IdentityUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public virtual async Task<bool> IsViewerLicense()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return false;

            var roles = await GetUserRolesAsync(currentUser);
            return roles.Contains("Viewer");
        }

        public virtual async Task<bool> IsPlannerLicense()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return false;

            var roles = await GetUserRolesAsync(currentUser);
            return roles.Contains("Planner");
        }

        public virtual async Task<bool> IsAdminLicense()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser is null)
                return false;

            var roles = await GetUserRolesAsync(currentUser);
            return roles.Contains("Admin");
        }
    }
}