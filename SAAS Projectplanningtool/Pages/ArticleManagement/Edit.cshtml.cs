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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        [BindProperty] public Article Article { get; set; } = default!;
        [BindProperty] public decimal Price { get; set; }
        [BindProperty] public string? PriceComment { get; set; }
        public decimal CurrentPrice { get; set; }
        public SelectList CategorySelectList { get; set; } = default!;
        public SelectList UnitSelectList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                if (id == null) return NotFound();

                var article = await _context.Article
                    .Include(a => a.ArticleCategory)
                    .Include(a => a.Unit)
                    .Include(a => a.PriceHistory)
                    .FirstOrDefaultAsync(a => a.ArticleId == id);

                if (article == null) return NotFound();
                Article = article;

                // Aktuellen Preis aus Historie laden
                CurrentPrice = article.CurrentPrice;
                Price = CurrentPrice;

                await LoadSelectListsAsync(article.CompanyId, article.ArticleCategoryId, article.UnitId);
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR: Article/Edit<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await LoadSelectListsAsync(Article.CompanyId, Article.ArticleCategoryId, Article.UnitId);

                if (!ModelState.IsValid) return Page();

                // Aktuellen Preis aus DB laden um Änderung zu erkennen
                var currentPriceEntry = await _context.ArticlePriceHistory
                    .Where(p => p.ArticleId == Article.ArticleId)
                    .OrderByDescending(p => p.Timestamp)
                    .FirstOrDefaultAsync();

                var oldPrice = currentPriceEntry?.Price ?? 0;

                // Preis hat sich geändert → neuen Eintrag schreiben
                if (Price != oldPrice)
                {
                    var employee = await new CustomUserManager(_context, _userManager)
                        .GetEmployeeAsync(_userManager.GetUserId(User));

                    var priceEntry = new ArticlePriceHistory
                    {
                        ArticleId = Article.ArticleId,
                        Price = Price,
                        Timestamp = DateTime.Now,
                        Comment = !string.IsNullOrWhiteSpace(PriceComment) ? PriceComment : null,
                        CreatedById = employee.EmployeeId
                    };
                    _context.ArticlePriceHistory.Add(priceEntry);
                }

                Article = await new CustomObjectModifier(_context, _userManager)
                    .AddLatestModificationAsync(User, "Artikel geändert", Article, false);

                _context.Attach(Article).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Article.Any(a => a.ArticleId == Article.ArticleId))
                        return NotFound();
                    throw;
                }

                return RedirectToPage("./Details", new { id = Article.ArticleId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, Article, "ERROR: Article/Edit<OnPost>") });
            }
        }

        private async Task LoadSelectListsAsync(string? companyId, string? selectedCategoryId, string? selectedUnitId)
        {
            var cats = await _context.ArticleCategory
                .Where(c => c.CompanyId == companyId && !c.DeleteFlag)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            CategorySelectList = new SelectList(cats, nameof(ArticleCategory.ArticleCategoryId),
                nameof(ArticleCategory.CategoryName), selectedCategoryId);

            // Aktive Company-Units laden
            var activeUnits = await _context.CompanyUnit
                .Where(cu => cu.CompanyId == companyId)
                .Include(cu => cu.Unit)
                .Select(cu => cu.Unit!)
                .OrderBy(u => u.Name)
                .ToListAsync();

            // Aktuell zugewiesene Unit einschließen, auch wenn deaktiviert
            if (!string.IsNullOrWhiteSpace(selectedUnitId) &&
                !activeUnits.Any(u => u.UnitId == selectedUnitId))
            {
                var currentUnit = await _context.Unit.FindAsync(selectedUnitId);
                if (currentUnit != null)
                {
                    activeUnits.Add(currentUnit);
                    activeUnits = activeUnits.OrderBy(u => u.Name).ToList();
                }
            }

            UnitSelectList = new SelectList(
                activeUnits.Select(u => new { u.UnitId, Display = $"{u.Name} ({u.ShortName})" }),
                "UnitId", "Display", selectedUnitId);
        }
    }
}
