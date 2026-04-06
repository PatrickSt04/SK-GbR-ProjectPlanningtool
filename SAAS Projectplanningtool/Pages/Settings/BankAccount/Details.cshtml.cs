using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAAS_Projectplanningtool.Data;

namespace SAAS_Projectplanningtool.Pages.Settings.BankAccount
{
    public class DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        : BankAccountPageModel(context, userManager)
    {
        public Models.BankAccount BankAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/BankAccount/Details<OnGet>Begin");
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var bankAccount = await FindBankAccountForCurrentCompanyAsync(id, includeAuditInformation: true);
                if (bankAccount == null)
                {
                    return NotFound();
                }

                BankAccount = bankAccount;
                await Logger.Log(null, User, BankAccount, "Settings/BankAccount/Details<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, null, "ERROR:Settings/BankAccount/Details<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostSetDeleteFlagAsync(string id)
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/BankAccount/Details<OnPostSetDeleteFlag>Begin");
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var bankAccount = await FindBankAccountForCurrentCompanyAsync(id);
                if (bankAccount == null)
                {
                    return NotFound();
                }

                bankAccount = await ObjectModifier.SetDeleteFlagAsync(true, bankAccount, User);
                Context.BankAccount.Update(bankAccount);
                await Context.SaveChangesAsync();

                await Logger.Log(null, User, bankAccount, "Settings/BankAccount/Details<OnPostSetDeleteFlag>End");
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, null, "ERROR:Settings/BankAccount/Details<OnPostSetDeleteFlag>") });
            }
        }

        public async Task<IActionResult> OnPostUndoDeleteFlagAsync(string id)
        {
            try
            {
                await Logger.Log(null, User, null, "Settings/BankAccount/Details<OnPostUndoDeleteFlag>Begin");
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var bankAccount = await FindBankAccountForCurrentCompanyAsync(id);
                if (bankAccount == null)
                {
                    return NotFound();
                }

                bankAccount = await ObjectModifier.SetDeleteFlagAsync(false, bankAccount, User);
                Context.BankAccount.Update(bankAccount);
                await Context.SaveChangesAsync();

                await Logger.Log(null, User, bankAccount, "Settings/BankAccount/Details<OnPostUndoDeleteFlag>End");
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await Logger.Log(ex, User, null, "ERROR:Settings/BankAccount/Details<OnPostUndoDeleteFlag>") });
            }
        }
    }
}
