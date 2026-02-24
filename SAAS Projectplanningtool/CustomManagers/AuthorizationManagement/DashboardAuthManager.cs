using System;

namespace SAAS_Projectplanningtool.CustomManagers.AuthorizationManagement.ProjectAuthorizationManagement
{
    public class DashboardAuthManager : AuthorizationManager
    {
        public DashboardAuthManager(object userManager, object roleManager, object context)
            : base(userManager, roleManager, context)
        {
        }

        public virtual bool ViewerDashboard()
        {
            throw new NotImplementedException();
        }

        public virtual bool PlannerDashboard()
        {
            throw new NotImplementedException();
        }

        public virtual bool AdminDashboard()
        {
            throw new NotImplementedException();
        }
    }
}