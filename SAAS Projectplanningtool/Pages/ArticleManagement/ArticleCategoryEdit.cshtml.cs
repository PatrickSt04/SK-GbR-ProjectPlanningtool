using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.ArticleManagement;

namespace SAAS_Projectplanningtool.Pages.ArticleManagement
{
    public class ArticleCategoryEditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public ArticleCategoryEditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        [BindProperty] public ArticleCategory Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) return NotFound();

            var category = await _context.ArticleCategory.FindAsync(id);
            if (category == null) return NotFound();

            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid) return Page();

                Category = await new CustomObjectModifier(_context, _userManager)
                    .AddLatestModificationAsync(User, "Kategorie geändert", Category, false);

                _context.Attach(Category).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToPage("./ArticleCategoryDetails", new { id = Category.ArticleCategoryId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, Category, "ERROR: ArticleCategory/Edit<OnPost>") });
            }
        }
    }
}
