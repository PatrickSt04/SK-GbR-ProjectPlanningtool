using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class ProjectDetailsAuthManager : AuthorizationManager
    {
        public ProjectDetailsAuthManager(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ClaimsPrincipal user)
            : base(userManager, roleManager, context, user)
        {
        }

        public virtual async Task<bool> CanArchiveProject()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewClosedTasks()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewProgress()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewStartEndDate()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewProjectInformation()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewProjectStructure()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanViewOwnTimeEntries()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> CanTrackOwnTime()
        {
            throw new NotImplementedException();
        }
    }
}