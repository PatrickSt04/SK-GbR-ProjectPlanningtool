using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectIndexAuthManager(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        ClaimsPrincipal user)
        : AuthorizationManager(userManager, roleManager, context, user)
    {

        public virtual async Task<bool> CanViewProjectBudgetInformation()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanViewProjectInformation()
        {
            return await IsAdminLicense() || await IsPlannerLicense() || await IsViewerLicense();
        }

        public virtual async Task<bool> CanViewActiveProjects()
        {
            return await IsAdminLicense() || await IsPlannerLicense()  || await IsViewerLicense(); 
        }

        public virtual async Task<bool> CanViewArchivedProjects()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanViewPlannedProjects()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanViewDueProjects()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }
    }
}