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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomUserManager customUserManager;

        public Logger(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            customUserManager = new CustomUserManager(context, userManager);
        }

        public async Task Log(Exception exception, ClaimsPrincipal ExcecutingUserTable, object UsedModelObject )
        {
            var excecutingEmployee = await customUserManager.GetEmployeeAsync(_userManager.GetUserId(ExcecutingUserTable));
            var log = new Logfile { ExceptionName = exception.GetType().FullName,               // NullReferenceException
                ExceptionMessage = exception.Message,                                           // Object reference not set to an instance of an object.
                ExceptionPath = exception.TargetSite.DeclaringType.FullName,                    // SAAS_Projectplanningtool.Pages.Projects.DetailsModel+<OnGetAsync>d__15
                ExcecutingEmployee = excecutingEmployee,                                        // Employee
                TimeOfException = DateTime.Now,                                                 // 2021-09-15 12:00:00
                SerializedObject = JsonSerializer.Serialize(UsedModelObject)                    // Project
            };
            _context.Logfile.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
