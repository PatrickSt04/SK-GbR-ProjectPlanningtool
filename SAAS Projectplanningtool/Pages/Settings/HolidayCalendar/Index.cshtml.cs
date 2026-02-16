using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.Settings.HolidayCalendar
{
    public class IndexModel : HolidayCalendarPageModel
    {
        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        public IList<HolidayCalendarEntry> HolidayEntries { get; set; } = new List<HolidayCalendarEntry>();
        public string CompanyName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/HolidayCalendar/Index<OnGet>Begin");

                var employee = await GetCurrentEmployeeAsync();
                if (employee?.CompanyId == null)
                {
                    return NotFound();
                }

                CompanyName = employee.Company?.CompanyName ?? string.Empty;

                HolidayEntries = await CompanyScopedHolidays(employee.CompanyId)
                    .Include(h => h.CreatedByEmployee)
                    .Include(h => h.LatestModifier)
                    .OrderBy(h => h.HolidayDate)
                    .ThenBy(h => h.HolidayName)
                    .ToListAsync();

                await Logger.Log(null, User, null, "Settings/HolidayCalendar/Index<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, null, "ERROR:Settings/HolidayCalendar/Index<OnGet>") });
            }
        }
    }
}
