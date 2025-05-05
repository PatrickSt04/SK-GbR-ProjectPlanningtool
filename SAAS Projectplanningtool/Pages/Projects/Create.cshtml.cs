using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class CreateModel : ProjectPageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        public CreateModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }
        [BindProperty]
        public Project Project { get; set; } = default!;
        public float InitialBudget { get; set; } = 0.0f;
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/CreateModel<OnGetAsync>Beginn");
                await PublishCustomersAsync();
                await PublishProjectLeadsAsync();
                await _logger.Log(null, User, null, "Projects/CreateModel<OnGetAsync>End");
                return Page();
            }
            catch(Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Project, null) });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/CreateModel<OnGetAsync>Beginn");
                if (!ModelState.IsValid)
                {
                    await PublishCustomersAsync();
                    await PublishProjectLeadsAsync();
                    return Page();
                }
                //companyuser lesen
                var companyuser = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));

                //Budget Eintrag hinzufügen
                var projectBudget = new ProjectBudget
                {
                    InitialBudget = InitialBudget,
                    CompanyId = companyuser.CompanyId,
                };
                projectBudget = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Projektbudget angelegt", projectBudget, true);
                Project.ProjectBudget = projectBudget;
                Project.CompanyId = companyuser.CompanyId;

                // LatestModification Attribut hinzufügen
                Project = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User,"Projekt angelegt", Project, true);

                _context.Project.Add(Project);
                await _context.SaveChangesAsync();


                await _logger.Log(null, User, null, "Projects/CreateModel<OnGetAsync>End");
                return RedirectToPage("/Projects/Scheduling", new {id = Project.ProjectId});
            }catch (Exception ex) {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Project, null) });
            }
        }
    }
}
