using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.ArticleManagement;

namespace SAAS_Projectplanningtool.Pages.ArticleManagement
{
    public class CreateModel(
       ApplicationDbContext context,
       UserManager<IdentityUser> userManager) : PageModel
    {
        private readonly Logger _logger = new(context, userManager);

        [BindProperty] public Article Article { get; set; } = default!;
        [BindProperty] public decimal Price { get; set; }
        public SelectList CategorySelectList { get; set; } = default!;
        public SelectList UnitSelectList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? categoryId = null)
        {
            try
            {
                var employee = await new CustomUserManager(context, userManager)
                    .GetEmployeeAsync(userManager.GetUserId(User));

                Article = new Article
                {
                    ArticleNumber = "",
                    ArticleName = "",
                    CompanyId = employee.CompanyId,
                    ArticleCategoryId = categoryId,
                    UnitId = ""
                };

                await LoadSelectListsAsync(employee.CompanyId, categoryId, null);
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR: Article/Create<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var employee = await new CustomUserManager(context, userManager)
                    .GetEmployeeAsync(userManager.GetUserId(User));

                if (!ModelState.IsValid)
                {
                    await LoadSelectListsAsync(employee.CompanyId, Article.ArticleCategoryId, null);
                    return Page();
                }

                Article = await new CustomObjectModifier(context, userManager)
                    .AddLatestModificationAsync(User, "Artikel angelegt", Article, true);

                context.Article.Add(Article);

                // Ersten Preis-Eintrag in die Historie schreiben
                var priceEntry = new ArticlePriceHistory
                {
                    ArticleId = Article.ArticleId,
                    Price = Price,
                    Timestamp = DateTime.Now,
                    Comment = "Erstanlage",
                    CreatedById = employee.EmployeeId
                };
                context.ArticlePriceHistory.Add(priceEntry);

                await context.SaveChangesAsync();

                return RedirectToPage("./Details", new { id = Article.ArticleId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, Article, "ERROR: Article/Create<OnPost>") });
            }
        }

        private async Task LoadSelectListsAsync(string? companyId, string? selectedCategoryId, string? selectedUnitId)
        {
            var cats = await context.ArticleCategory
                .Where(c => c.CompanyId == companyId && !c.DeleteFlag)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            CategorySelectList = new SelectList(cats, nameof(ArticleCategory.ArticleCategoryId),
                nameof(ArticleCategory.CategoryName), selectedCategoryId);

            // Nur aktive Company-Units laden
            var units = await context.CompanyUnit
                .Where(cu => cu.CompanyId == companyId)
                .Include(cu => cu.Unit)
                .Select(cu => cu.Unit!)
                .OrderBy(u => u.Name)
                .ToListAsync();

            UnitSelectList = new SelectList(
                units.Select(u => new { u.UnitId, Display = $"{u.Name} ({u.ShortName})" }),
                "UnitId", "Display", selectedUnitId);
        }
    }
}
