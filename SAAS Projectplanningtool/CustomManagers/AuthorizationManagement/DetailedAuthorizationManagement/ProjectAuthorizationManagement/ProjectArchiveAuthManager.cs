using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectArchiveAuthManager : AuthorizationManager
    {
        public ProjectArchiveAuthManager(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ClaimsPrincipal user)
            : base(userManager, roleManager, context, user)
        {
        }

        public virtual async Task<bool> CanArchiveProject()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanUnarchiveProject()
        {
            throw new NotImplementedException();
        }
    }
}