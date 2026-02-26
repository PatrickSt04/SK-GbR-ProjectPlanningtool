using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectArchiveAuthManager(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        ClaimsPrincipal user)
        : AuthorizationManager(userManager, roleManager, context, user)
    {
        public virtual async Task<bool> CanArchiveProject()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }
        
        public virtual async Task<bool> CanUnarchiveProject()
        {
            return await IsAdminLicense();
        }
    }
}