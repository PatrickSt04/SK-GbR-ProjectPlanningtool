using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.Settings.HolidayCalendar
{
    public abstract class HolidayCalendarPageModel : PageModel
    {
        protected readonly ApplicationDbContext Context;
        protected readonly UserManager<IdentityUser> UserManager;
        protected readonly Logger Logger;
        protected readonly CustomObjectModifier ObjectModifier;
        private readonly CustomUserManager _customUserManager;

        protected HolidayCalendarPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            Context = context;
            UserManager = userManager;
            Logger = new Logger(context, userManager);
            ObjectModifier = new CustomObjectModifier(context, userManager);
            _customUserManager = new CustomUserManager(context, userManager);
        }

        protected async Task<Employee?> GetCurrentEmployeeAsync()
        {
            var userId = UserManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await _customUserManager.GetEmployeeAsync(userId);
        }

        protected IQueryable<HolidayCalendarEntry> CompanyScopedHolidays(string companyId)
        {
            return Context.HolidayCalendarEntry.Where(h => h.CompanyId == companyId);
        }

        protected async Task<HolidayCalendarEntry?> FindHolidayForCurrentCompanyAsync(string holidayId, bool includeAuditInformation = false)
        {
            var employee = await GetCurrentEmployeeAsync();
            if (employee?.CompanyId == null)
            {
                return null;
            }

            var query = CompanyScopedHolidays(employee.CompanyId);
            if (includeAuditInformation)
            {
                query = query
                    .Include(h => h.CreatedByEmployee)
                    .Include(h => h.LatestModifier);
            }

            return await query.FirstOrDefaultAsync(h => h.HolidayCalendarEntryId == holidayId);
        }
    }
}
