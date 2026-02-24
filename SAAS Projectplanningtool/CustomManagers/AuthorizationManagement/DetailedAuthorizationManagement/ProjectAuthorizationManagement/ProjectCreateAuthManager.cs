using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectCreateAuthManager : AuthorizationManager
    {
        public ProjectCreateAuthManager(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ClaimsPrincipal user)
            : base(userManager, roleManager, context, user)
        {
        }

        public virtual async Task<bool> CanCreateProject()
        {
            throw new NotImplementedException();
        }
    }
}