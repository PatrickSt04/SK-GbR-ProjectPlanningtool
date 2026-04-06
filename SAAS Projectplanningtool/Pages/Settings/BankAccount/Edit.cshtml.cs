using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAAS_Projectplanningtool.Data;

namespace SAAS_Projectplanningtool.Pages.Settings.BankAccount
{
    public class EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        : BankAccountPageModel(context, userManager)
    {
        [BindProperty]
        public Models.BankAccount BankAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/BankAccount/Edit<OnGet>Begin");
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var bankAccount = await FindBankAccountForCurrentCompanyAsync(id);
                if (bankAccount == null)
                {
                    return NotFound();
                }

                BankAccount = bankAccount;
                await Logger.Log(null, User, BankAccount, "Settings/BankAccount/Edit<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, BankAccount, "ERROR:Settings/BankAccount/Edit<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await Logger.Log(null, User, BankAccount, "Settings/BankAccount/Edit<OnPost>Begin");
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var existing = await FindBankAccountForCurrentCompanyAsync(BankAccount.BankAccountId);
                if (existing == null)
                {
                    return NotFound();
                }

                existing.AccountName = BankAccount.AccountName;
                existing.IBAN = BankAccount.IBAN;
                existing.BIC = BankAccount.BIC;
                existing.BankName = BankAccount.BankName;
                existing.AccountHolder = BankAccount.AccountHolder;
                existing = await ObjectModifier.AddLatestModificationAsync(User, "Bankkonto geändert", existing, false);

                Context.BankAccount.Update(existing);
                await Context.SaveChangesAsync();

                await Logger.Log(null, User, existing, "Settings/BankAccount/Edit<OnPost>End");
                return RedirectToPage("./Details", new { id = existing.BankAccountId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, BankAccount, "ERROR:Settings/BankAccount/Edit<OnPost>") });
            }
        }
    }
}
