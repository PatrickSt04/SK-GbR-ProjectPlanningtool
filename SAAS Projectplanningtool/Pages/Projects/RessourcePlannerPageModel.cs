using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class RessourcePlannerPageModel : ProjectPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        //Diese Klasse wird in den Razor Pages verwendet, um im Terminplan Ressourcen einzubinden.
        public RessourcePlannerPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }
        //This Method adds HourlyRateGroups to the Task
        public async Task<IActionResult> OnPostAddHourlyRateGroupsAsync(string projectId, Dictionary<int, string > HourlyRateGroups)
        {

            return RedirectToPage(new { id = projectId });
        }

    }
}
