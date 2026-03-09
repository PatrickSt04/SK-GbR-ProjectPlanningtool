using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.ArticleManagement;

namespace SAAS_Projectplanningtool.Pages.ArticleManagement
{
    public class ArticleCategoryCreateModel(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager) : PageModel
    {
        private readonly Logger _logger = new(context, userManager);

        [BindProperty] public ArticleCategory Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var employee = await new CustomUserManager(context, userManager)
                    .GetEmployeeAsync(userManager.GetUserId(User));

                Category = new ArticleCategory
                {
                    CategoryName = "",
                    CompanyId = employee.CompanyId
                };
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR: ArticleCategory/Create<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid) return Page();

                Category = await new CustomObjectModifier(context, userManager)
                    .AddLatestModificationAsync(User, "Artikelkategorie angelegt", Category, true);

                context.ArticleCategory.Add(Category);
                await context.SaveChangesAsync();

                return RedirectToPage("./ArticleCategoryDetails", new { id = Category.ArticleCategoryId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, Category, "ERROR: ArticleCategory/Create<OnPost>") });
            }
        }
    }
}
