using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.Settings.HolidayCalendar
{
    public class DetailsModel : HolidayCalendarPageModel
    {
        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        public HolidayCalendarEntry HolidayCalendarEntry { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/HolidayCalendar/Details<OnGet>Begin");
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var holidayEntry = await FindHolidayForCurrentCompanyAsync(id, includeAuditInformation: true);
                if (holidayEntry == null)
                {
                    return NotFound();
                }

                HolidayCalendarEntry = holidayEntry;
                await Logger.Log(null, User, HolidayCalendarEntry, "Settings/HolidayCalendar/Details<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, null, "ERROR:Settings/HolidayCalendar/Details<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostSetDeleteFlagAsync(string id)
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/HolidayCalendar/Details<OnPostSetDeleteFlag>Begin");
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var holidayEntry = await FindHolidayForCurrentCompanyAsync(id);
                if (holidayEntry == null)
                {
                    return NotFound();
                }

                holidayEntry = await ObjectModifier.SetDeleteFlagAsync(true, holidayEntry, User);
                Context.HolidayCalendarEntry.Update(holidayEntry);
                await Context.SaveChangesAsync();

                await Logger.Log(null, User, holidayEntry, "Settings/HolidayCalendar/Details<OnPostSetDeleteFlag>End");
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, null, "ERROR:Settings/HolidayCalendar/Details<OnPostSetDeleteFlag>") });
            }
        }

        public async Task<IActionResult> OnPostUndoDeleteFlagAsync(string id)
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/HolidayCalendar/Details<OnPostUndoDeleteFlag>Begin");
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var holidayEntry = await FindHolidayForCurrentCompanyAsync(id);
                if (holidayEntry == null)
                {
                    return NotFound();
                }

                holidayEntry = await ObjectModifier.SetDeleteFlagAsync(false, holidayEntry, User);
                Context.HolidayCalendarEntry.Update(holidayEntry);
                await Context.SaveChangesAsync();

                await Logger.Log(null, User, holidayEntry, "Settings/HolidayCalendar/Details<OnPostUndoDeleteFlag>End");
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, null, "ERROR:Settings/HolidayCalendar/Details<OnPostUndoDeleteFlag>") });
            }
        }
    }
}
