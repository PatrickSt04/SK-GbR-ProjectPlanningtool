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
    public class _SectionNodeModel(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        : PageModel
    {
        private UserManager<IdentityUser> _userManager = userManager;
        private ApplicationDbContext _context = context;
        private Logger _logger = new(context, userManager);

        public async Task OnGet()
        {

        }
    }
}

