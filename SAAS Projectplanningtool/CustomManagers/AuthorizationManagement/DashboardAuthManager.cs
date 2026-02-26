using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class DashboardAuthManager(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        ClaimsPrincipal user)
        : AuthorizationManager(userManager, roleManager, context, user)
    {
        public virtual async Task<bool> ViewerDashboard()
        {
            return await IsViewerLicense();
        }

        public virtual async Task<bool> PlannerDashboard()
        {
            return await IsPlannerLicense();
        }

        public virtual async Task<bool> AdminDashboard()
        {
            return await IsAdminLicense();
        }
    }
}