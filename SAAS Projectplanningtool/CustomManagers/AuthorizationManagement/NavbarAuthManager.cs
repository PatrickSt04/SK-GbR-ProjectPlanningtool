using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class NavbarAuthManager(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        ClaimsPrincipal user)
        : AuthorizationManager(userManager, roleManager, context, user)
    {
        public virtual bool DisplayProjectsNav()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> DisplayResourceNav()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> DisplayCustomerNav()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }

        public virtual async Task<bool> DisplaySettingsNav()
        {
            return await IsAdminLicense() || await IsPlannerLicense();
        }
    }
}