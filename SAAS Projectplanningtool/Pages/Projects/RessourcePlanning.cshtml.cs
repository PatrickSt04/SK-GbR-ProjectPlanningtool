using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class RessourcePlanningModel : RessourcePlannerPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        public RessourcePlanningModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(context, userManager);
        }
        [BindProperty]
        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await _logger.Log(null, User, null, "Projects.RessourcePlanning<OnGetAsync>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var project = await GetProjectAsync(id);
                if (project == null)
                {
                    return NotFound();
                }
                Project = project;
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, null));
            }
            await _logger.Log(null, User, null, "Projects.RessourcePlanning<OnGetAsync>End");
            return Page();
        }

    }
}
