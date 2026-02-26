using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.ObjectPool;
using Microsoft.VisualStudio.Web.CodeGeneration;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers
{
    public class CustomObjectModifier(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly CustomUserManager _customUserManager = new(context, userManager);

        // Diese Methode fügt dem Objekt werte für LatestModification Attribute hinzu
        // Es können exceptions geworfen werden. Diese müssen in der aufrufenden Methode behandelt werden ( Logger -> Logfile )
        public async Task<TObject> AddLatestModificationAsync<TObject>(ClaimsPrincipal ExcecutingUserTable, string ModificationText, TObject ModifiedObject, bool isFirtCreateOfObject)
        {
            var employee = await _customUserManager.GetEmployeeAsync(userManager.GetUserId(ExcecutingUserTable));
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
                property.SetValue(ModifiedObject, ModificationText);
            }
            if (!isFirtCreateOfObject)
            {
                // zurückgeben des modifizierten Objekts

                return ModifiedObject;
            }

            //Creating User Attribut holen
            property = ModifiedObject.GetType().GetProperty("CreatedByEmployee");
            //Creating User Attribut setzen
            if (property != null && property.CanWrite && property.PropertyType == typeof(Employee))
            {
                property.SetValue(ModifiedObject, employee);
            }
            //Creating Timestamp Attribut holen
            property = ModifiedObject.GetType().GetProperty("CreatedTimestamp");
            //Creating Timestamp Attribut setzen
            if (property != null && property.CanWrite &&
                (property.PropertyType == typeof(DateTime) ||
                 Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime)))
            {
                property.SetValue(ModifiedObject, DateTime.Now);
            }



            // zurückgeben des modifizierten Objekts

            return ModifiedObject;
        }
        public async Task<TObject> SetDeleteFlagAsync<TObject>(bool DeleteFlag, TObject ModifiedObject, ClaimsPrincipal User)
        {
            PropertyInfo property = ModifiedObject.GetType().GetProperty("DeleteFlag");
            if (property != null && property.CanWrite)
            {
                property.SetValue(ModifiedObject, DeleteFlag);
            }
            ModifiedObject = await AddLatestModificationAsync(User, "Löschkennzeichen gesetzt", ModifiedObject, false);

            return ModifiedObject;
        }
    }
}
