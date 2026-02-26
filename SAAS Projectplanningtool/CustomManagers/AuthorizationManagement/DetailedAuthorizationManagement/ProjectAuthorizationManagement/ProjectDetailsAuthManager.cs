using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectDetailsAuthManager(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        ClaimsPrincipal user)
        : AuthorizationManager(userManager, roleManager, context, user)
    {
        public virtual async Task<bool> CanArchiveProject()
        {
            return await IsAdminLicense();
        }

        public virtual async Task<bool> CanViewClosedTasks()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanViewProgress()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanViewStartEndDate()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanViewProjectInformation()
        {
            return await IsAdminLicense() || await IsPlannerLicense() || await IsViewerLicense();
        }

        public virtual async Task<bool> CanViewProjectStructure()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanViewOwnTimeEntries()
        {
            return await IsAdminLicense() || await IsPlannerLicense()  || await IsViewerLicense();
        }

        public virtual async Task<bool> CanTrackOwnTime()
        {
            return await IsAdminLicense() || await IsPlannerLicense()  || await IsViewerLicense();
        }
    }
}