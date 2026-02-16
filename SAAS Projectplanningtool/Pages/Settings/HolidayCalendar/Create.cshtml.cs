using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.Settings.HolidayCalendar
{
    public class CreateModel : HolidayCalendarPageModel
    {
        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        [BindProperty]
        public HolidayCalendarEntry HolidayCalendarEntry { get; set; } = default!;

        public IReadOnlyList<string> SuggestedHolidayNames => HolidayCalendarDefaults.SuggestedHolidayNames;

        public async Task<IActionResult> OnGetAsync(DateOnly? date)
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/HolidayCalendar/Create<OnGet>Begin");

                var employee = await GetCurrentEmployeeAsync();
                if (employee?.CompanyId == null)
                {
                    return NotFound();
                }

                HolidayCalendarEntry = new HolidayCalendarEntry
                {
                    HolidayDate = date ?? DateOnly.FromDateTime(DateTime.Now),
                    HolidayType = HolidayEntryType.PublicHoliday
                };

                await Logger.Log(null, User, null, "Settings/HolidayCalendar/Create<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, null, "ERROR:Settings/HolidayCalendar/Create<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await Logger.Log(null, User, HolidayCalendarEntry, "Settings/HolidayCalendar/Create<OnPost>Begin");

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var employee = await GetCurrentEmployeeAsync();
                if (employee?.CompanyId == null)
                {
                    return NotFound();
                }

                HolidayCalendarEntry.CompanyId = employee.CompanyId;
                HolidayCalendarEntry = await ObjectModifier.AddLatestModificationAsync(User, "Feiertagseintrag angelegt", HolidayCalendarEntry, true);

                Context.HolidayCalendarEntry.Add(HolidayCalendarEntry);
                await Context.SaveChangesAsync();

                await Logger.Log(null, User, HolidayCalendarEntry, "Settings/HolidayCalendar/Create<OnPost>End");
                return RedirectToPage("./Details", new { id = HolidayCalendarEntry.HolidayCalendarEntryId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, HolidayCalendarEntry, "ERROR:Settings/HolidayCalendar/Create<OnPost>") });
            }
        }
    }
}
