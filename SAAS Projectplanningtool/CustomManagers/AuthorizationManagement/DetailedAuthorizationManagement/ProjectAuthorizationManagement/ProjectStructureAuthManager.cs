using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectStructureAuthManager(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        ClaimsPrincipal user)
        : AuthorizationManager(userManager, roleManager, context, user)
    {
        public virtual async Task<bool> CanViewProjectStructure()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanCreateTask()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanCreateSection()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanCreateMilestone()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanEditTask()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanEditSection()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanEditMilestone()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanDeleteTask()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanDeleteSection()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanDeleteMilestone()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }
    }
}