using Microsoft.AspNetCore.Identity;

namespace SAAS_Projectplanningtool.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
        }
    }
}
