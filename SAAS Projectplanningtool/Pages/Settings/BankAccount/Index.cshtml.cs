using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;

namespace SAAS_Projectplanningtool.Pages.Settings.BankAccount
{
    public class IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        : BankAccountPageModel(context, userManager)
    {
        public IList<Models.BankAccount> BankAccounts { get; set; } = new List<Models.BankAccount>();
        public string CompanyName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/BankAccount/Index<OnGet>Begin");

                var employee = await GetCurrentEmployeeAsync();
                if (employee?.CompanyId == null)
                {
                    return NotFound();
                }

                CompanyName = employee.Company?.CompanyName ?? string.Empty;

                BankAccounts = await CompanyScopedBankAccounts(employee.CompanyId)
                    .Include(b => b.CreatedByEmployee)
                    .Include(b => b.LatestModifier)
                    .OrderBy(b => b.AccountName)
                    .ToListAsync();

                await Logger.Log(null, User, null, "Settings/BankAccount/Index<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, null, "ERROR:Settings/BankAccount/Index<OnGet>") });
            }
        }
    }
}
