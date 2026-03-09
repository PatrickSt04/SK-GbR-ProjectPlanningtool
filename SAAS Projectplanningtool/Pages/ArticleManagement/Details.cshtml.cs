using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.ArticleManagement;

namespace SAAS_Projectplanningtool.Pages.ArticleManagement
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public Article Article { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) return NotFound();

            var article = await _context.Article
                .Include(a => a.ArticleCategory)
                .Include(a => a.LatestModifier)
                .Include(a => a.CreatedByEmployee)
                .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null) return NotFound();
            Article = article;
            return Page();
        }

        public async Task<IActionResult> OnPostSetDeleteFlagAsync(string id)
        {
            try
            {
                var article = await _context.Article.FindAsync(id);
                if (article == null) return NotFound();

                article = await new CustomObjectModifier(_context, _userManager)
                    .SetDeleteFlagAsync(true, article, User);
                _context.Article.Update(article);
                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR: Article/Details<SetDeleteFlag>") });
            }
        }

        public async Task<IActionResult> OnPostUndoDeleteFlagAsync(string id)
        {
            try
            {
                var article = await _context.Article.FindAsync(id);
                if (article == null) return NotFound();

                article = await new CustomObjectModifier(_context, _userManager)
                    .SetDeleteFlagAsync(false, article, User);
                _context.Article.Update(article);
                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR: Article/Details<UndoDeleteFlag>") });
            }
        }
    }
}
