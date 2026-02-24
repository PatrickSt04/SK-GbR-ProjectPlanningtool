using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectIndexAuthManager : AuthorizationManager
    {
        public ProjectIndexAuthManager(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ClaimsPrincipal user)
            : base(userManager, roleManager, context, user)
        {
        }

        public virtual async Task<List<string>> CanProjectsToView()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewProjectBudgetInformation()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewProjectInformation()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewActiveProjects()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewArchivedProjects()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewPlannedProjects()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewDueProjects()
        {
            throw new NotImplementedException();
        }
    }
}