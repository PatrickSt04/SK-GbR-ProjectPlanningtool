using System;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class NavbarAuthManager : AuthorizationManager
    {
        public NavbarAuthManager(object userManager, object roleManager, object context)
            : base(userManager, roleManager, context)
        {
        }

        public virtual bool DisplayProjectsNav()
        {
            throw new NotImplementedException();
        }

        public virtual bool DisplayResourceNav()
        {
            throw new NotImplementedException();
        }

        public virtual bool DisplayCustomerNav()
        {
            throw new NotImplementedException();
        }

        public virtual bool DisplaySettingsNav()
        {
            throw new NotImplementedException();
        }
    }
}