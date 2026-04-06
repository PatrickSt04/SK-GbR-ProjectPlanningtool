using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAAS_Projectplanningtool.Data;

namespace SAAS_Projectplanningtool.Pages.Settings.BankAccount
{
    public class CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        : BankAccountPageModel(context, userManager)
    {
        [BindProperty]
        public Models.BankAccount BankAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/BankAccount/Create<OnGet>Begin");

                var employee = await GetCurrentEmployeeAsync();
                if (employee?.CompanyId == null)
                {
                    return NotFound();
                }

                BankAccount = new Models.BankAccount();

                await Logger.Log(null, User, null, "Settings/BankAccount/Create<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, null, "ERROR:Settings/BankAccount/Create<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await Logger.Log(null, User, BankAccount, "Settings/BankAccount/Create<OnPost>Begin");

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var employee = await GetCurrentEmployeeAsync();
                if (employee?.CompanyId == null)
                {
                    return NotFound();
                }

                BankAccount.CompanyId = employee.CompanyId;
                BankAccount = await ObjectModifier.AddLatestModificationAsync(User, "Bankkonto angelegt", BankAccount, true);

                Context.BankAccount.Add(BankAccount);
                await Context.SaveChangesAsync();

                await Logger.Log(null, User, BankAccount, "Settings/BankAccount/Create<OnPost>End");
                return RedirectToPage("./Details", new { id = BankAccount.BankAccountId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, BankAccount, "ERROR:Settings/BankAccount/Create<OnPost>") });
            }
        }
    }
}
