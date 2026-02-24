using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectStructureAuthManager : AuthorizationManager
    {
        public ProjectStructureAuthManager(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ClaimsPrincipal user)
            : base(userManager, roleManager, context, user)
        {
        }

        public virtual async Task<bool> CanViewProjectStructure()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanCreateTask()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanCreateSection()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanCreateMilestone()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanEditTask()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanEditSection()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanEditMilestone()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanDeleteTask()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanDeleteSection()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanDeleteMilestone()
        {
            throw new NotImplementedException();
        }
    }
}