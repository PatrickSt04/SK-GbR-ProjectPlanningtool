using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class _SectionNodeModel : PageModel
    {
        private UserManager<IdentityUser> _userManager;
        private ApplicationDbContext _context;
        private Logger _logger;
        public _SectionNodeModel(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _logger = new Logger(context, userManager);
        }
        public async Task OnGet()
        {

        }
    }
}

