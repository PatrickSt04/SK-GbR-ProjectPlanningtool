using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectTaskCatalogAuthManager : AuthorizationManager
    {
        public ProjectTaskCatalogAuthManager(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ClaimsPrincipal user)
            : base(userManager, roleManager, context, user)
        {
        }

        public virtual async Task<bool> CanViewTaskCatalog()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanCreateTaskCatalog()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanUpdateTaskCatalog()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanDeleteTaskCatalog()
        {
            throw new NotImplementedException();
        }
    }
}