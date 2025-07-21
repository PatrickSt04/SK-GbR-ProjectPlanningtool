using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using System.Security.Claims;
using SAAS_Projectplanningtool.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace SAAS_Projectplanningtool.CustomManagers
{
    public class Logger
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomUserManager customUserManager;

        public Logger(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            customUserManager = new CustomUserManager(context, userManager);
        }

        public async Task<string> Log(Exception? exception, ClaimsPrincipal ExcecutingUserTable, object? UsedModelObject, string? custommessage)
        {
            var excecutingEmployee = await customUserManager.GetEmployeeAsync(_userManager.GetUserId(ExcecutingUserTable));
            var log = new Logfile
            {
                ExceptionName = exception == null ? "null" : exception.GetType().FullName,
                ExceptionMessage = exception == null ? "null" : exception.Message,
                ExceptionPath = exception == null ? "null" : exception.TargetSite?.DeclaringType?.FullName,
                ExcecutingEmployee = excecutingEmployee,
                TimeOfException = DateTime.Now,
                CustomMessage = custommessage,
                SerializedObject = UsedModelObject == null ? "null" : JsonSerializer.Serialize(UsedModelObject)
            };

            await _context.Logfile.AddAsync(log);
            await _context.SaveChangesAsync();

            return log.LogfileId;
        }
        // Overload for logging by user id
        // Used for Login / Logout actions of a user
        public async Task<string> LogByUserId(string userId, string? customMessage, object? UsedModelObject = null)
        {
            var excecutingEmployee = await customUserManager.GetEmployeeAsync(userId);
            var log = new Logfile
            {
                ExceptionName = "null",
                ExceptionMessage = "null",
                ExceptionPath = "Login/Logout",
                ExcecutingEmployee = excecutingEmployee,
                TimeOfException = DateTime.Now,
                CustomMessage = customMessage,
                SerializedObject = UsedModelObject == null ? "null" : JsonSerializer.Serialize(UsedModelObject)
            };

            await _context.Logfile.AddAsync(log);
            await _context.SaveChangesAsync();

            return log.LogfileId;
        }
    }
}
