using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;

namespace SAAS_Projectplanningtool.CustomManagers
{
    public class CustomVisibilityManager
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomVisibilityManager(ApplicationDbContext context, UserManager<IdentityUser> userManger)
        {
            _context = context;
            _userManager = userManger;
        }

        public async Task checkAuthorizedByCompany(string companyId, string userId)
        {
            var employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(userId);
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