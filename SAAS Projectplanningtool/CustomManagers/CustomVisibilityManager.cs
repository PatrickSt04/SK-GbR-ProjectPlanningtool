using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;

namespace SAAS_Projectplanningtool.CustomManagers
{
    public class CustomVisibilityManager(ApplicationDbContext context, UserManager<IdentityUser> userManger)
    {
        public async Task checkAuthorizedByCompany(string companyId, string userId)
        {
            var employee = await new CustomUserManager(context, userManger).GetEmployeeAsync(userId);
            if (employee == null)
            {
                throw new Exception("Sicht auf dieses Element verweigert");
            }

            //Hier muss noch eine Prüfung auf Rollen erfolgen --> Throw Exception mit Fehlermeldung; Exception type Exception; 

            if (!companyId.Equals(employee?.CompanyId))
            {
                throw new Exception("Sicht auf dieses Element verweigert");
            }
        }
    }
}