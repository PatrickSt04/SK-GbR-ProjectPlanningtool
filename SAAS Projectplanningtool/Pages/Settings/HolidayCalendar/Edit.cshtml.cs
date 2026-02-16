using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.Settings.HolidayCalendar
{
    public class EditModel : HolidayCalendarPageModel
    {
        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        [BindProperty]
        public HolidayCalendarEntry HolidayCalendarEntry { get; set; } = default!;

        public IReadOnlyList<string> SuggestedHolidayNames => HolidayCalendarDefaults.SuggestedHolidayNames;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/HolidayCalendar/Edit<OnGet>Begin");
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var holidayEntry = await FindHolidayForCurrentCompanyAsync(id);
                if (holidayEntry == null)
                {
                    return NotFound();
                }

                HolidayCalendarEntry = holidayEntry;
                await Logger.Log(null, User, HolidayCalendarEntry, "Settings/HolidayCalendar/Edit<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, HolidayCalendarEntry, "ERROR:Settings/HolidayCalendar/Edit<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await Logger.Log(null, User, HolidayCalendarEntry, "Settings/HolidayCalendar/Edit<OnPost>Begin");
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var existingHoliday = await FindHolidayForCurrentCompanyAsync(HolidayCalendarEntry.HolidayCalendarEntryId);
                if (existingHoliday == null)
                {
                    return NotFound();
                }

                existingHoliday.HolidayName = HolidayCalendarEntry.HolidayName;
                existingHoliday.HolidayDate = HolidayCalendarEntry.HolidayDate;
                existingHoliday.HolidayType = HolidayCalendarEntry.HolidayType;
                existingHoliday = await ObjectModifier.AddLatestModificationAsync(User, "Feiertagseintrag geändert", existingHoliday, false);

                Context.HolidayCalendarEntry.Update(existingHoliday);
                await Context.SaveChangesAsync();

                await Logger.Log(null, User, existingHoliday, "Settings/HolidayCalendar/Edit<OnPost>End");
                return RedirectToPage("./Details", new { id = existingHoliday.HolidayCalendarEntryId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, HolidayCalendarEntry, "ERROR:Settings/HolidayCalendar/Edit<OnPost>") });
            }
        }
    }
}
