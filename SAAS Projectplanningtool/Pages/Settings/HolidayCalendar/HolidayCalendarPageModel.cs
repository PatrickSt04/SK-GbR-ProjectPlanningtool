using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.Settings.HolidayCalendar
{
    public abstract class HolidayCalendarPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        : PageModel
    {
        protected readonly ApplicationDbContext Context = context;
        protected readonly UserManager<IdentityUser> UserManager = userManager;
        protected readonly Logger Logger = new(context, userManager);
        protected readonly CustomObjectModifier ObjectModifier = new(context, userManager);
        private readonly CustomUserManager _customUserManager = new(context, userManager);

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
