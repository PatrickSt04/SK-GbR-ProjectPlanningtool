using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.ArticleManagement;

namespace SAAS_Projectplanningtool.Pages.ArticleManagement
{
    public class ArticleCategoryDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public ArticleCategoryDetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public ArticleCategory Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) return NotFound();

            var category = await _context.ArticleCategory
                .Include(c => c.Articles)
                .Include(c => c.LatestModifier)
                .Include(c => c.CreatedByEmployee)
                .Include(c => c.Articles)
                .ThenInclude(a => a.PriceHistory)
                .FirstOrDefaultAsync(c => c.ArticleCategoryId == id);

            if (category == null) return NotFound();
            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostSetDeleteFlagAsync(string id)
        {
            try
            {
                var category = await _context.ArticleCategory.FindAsync(id);
                if (category == null) return NotFound();

                category = await new CustomObjectModifier(_context, _userManager)
                    .SetDeleteFlagAsync(true, category, User);
                _context.ArticleCategory.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR: ArticleCategory/Details<SetDeleteFlag>") });
            }
        }

        public async Task<IActionResult> OnPostUndoDeleteFlagAsync(string id)
        {
            try
            {
                var category = await _context.ArticleCategory.FindAsync(id);
                if (category == null) return NotFound();

                category = await new CustomObjectModifier(_context, _userManager)
                    .SetDeleteFlagAsync(false, category, User);
                _context.ArticleCategory.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR: ArticleCategory/Details<UndoDeleteFlag>") });
            }
        }
    }
}
