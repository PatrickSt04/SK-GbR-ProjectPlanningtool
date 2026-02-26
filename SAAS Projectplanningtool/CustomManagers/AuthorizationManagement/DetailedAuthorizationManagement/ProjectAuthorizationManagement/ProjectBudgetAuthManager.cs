using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectBudgetAuthManager(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        ClaimsPrincipal user)
        : AuthorizationManager(userManager, roleManager, context, user)
    {
        public virtual async Task<bool> CanViewTimeEntries()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanPlanInitialBudget()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanTrackTimes()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> CanViewBudgetStatistics()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }
    }
}