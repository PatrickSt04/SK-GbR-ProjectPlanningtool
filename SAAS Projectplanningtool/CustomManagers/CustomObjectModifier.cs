using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGeneration;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers
{
    public class CustomObjectModifier
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomUserManager _customUserManager;

        public CustomObjectModifier(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _customUserManager = new CustomUserManager(context, userManager);
        }
        // Diese Methode fügt dem Objekt werte für LatestModification Attribute hinzu
        // Es können exceptions geworfen werden. Diese müssen in der aufrufenden Methode behandelt werden ( Logger -> Logfile )
        public async Task<TObject> AddLatestModificationAsync<TObject>(ClaimsPrincipal ExcecutingUserTable, string ModificationText, TObject ModifiedObject)
        {
            var employee = await _customUserManager.GetEmployeeAsync(_userManager.GetUserId(ExcecutingUserTable));
            //LatestModifier Attribut holen
            PropertyInfo property = ModifiedObject.GetType().GetProperty("LatestModifier");
            //LatestModifier Attribut setzen
            if (property != null && property.CanWrite && property.PropertyType == typeof(Employee))
            {
                property.SetValue(ModifiedObject, employee);
            }
            //LatestModificationTimestamp Attribut holen
            property = ModifiedObject.GetType().GetProperty("LatestModificationTimestamp");
            //LatestModificationTimestamp Attribut setzen
            if (property != null && property.CanWrite)
            {
                property.SetValue(ModifiedObject, DateTime.Now);
            }
            //LatestModificationText Attribut holen
            property = ModifiedObject.GetType().GetProperty("LatestModificationText");
            //LatestModificationText Attribut setzen
            if (property != null && property.CanWrite && property.PropertyType == typeof(string))
            {
                property.SetValue(ModifiedObject, "Letzter Änderer: " + employee.EmployeeDisplayName == null ? "unbekannt" : employee.EmployeeDisplayName +
                                                   "\n" + "Änderung" + ModificationText);
            }
            // zurückgeben des modifizierten Objekts
            return ModifiedObject;
        }

    }
}
