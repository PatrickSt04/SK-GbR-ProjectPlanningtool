using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectBudgetAuthManager : AuthorizationManager
    {
        public ProjectBudgetAuthManager(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ClaimsPrincipal user)
            : base(userManager, roleManager, context, user)
        {
        }

        public virtual async Task<bool> CanViewTimeEntries()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanPlanInitialBudget()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanTrackTimes()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewBudgetStatistics()
        {
            throw new NotImplementedException();
        }
    }
}