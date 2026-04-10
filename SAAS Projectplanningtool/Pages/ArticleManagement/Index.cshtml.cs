using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.ArticleManagement;

namespace SAAS_Projectplanningtool.Pages.ArticleManagement
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public IList<Article> Articles { get; set; } = default!;
        public IList<ArticleCategory> Categories { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var employee = await new CustomUserManager(_context, _userManager)
                    .GetEmployeeAsync(_userManager.GetUserId(User));

                Articles = await _context.Article
                    .Include(a => a.ArticleCategory)
                    .Include(a => a.Unit)
                    .Include(a => a.LatestModifier)
                    .Where(a => a.CompanyId == employee.CompanyId)
                    .OrderBy(a => a.ArticleNumber)
                    .ToListAsync();

                Categories = await _context.ArticleCategory
                    .Where(c => c.CompanyId == employee.CompanyId && !c.DeleteFlag)
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();

                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR: Article/Index<OnGet>") });
            }
        }
    }
}
