using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.CustomManagers;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class DetailsModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public DetailsModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Project Project { get; set; } = default!;
        public int TotalTasks { get; set; } = 0;
        public int CompletetedTasks { get; set; } = 0;  
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.LatestModidier)
                .Include(p => p.Company)
                .Include(p => p.Customer)
                .Include(p => p.ProjectBudget)
                .Include(p => p.ResponsiblePerson)
                .Include(p => p.State)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                Project = project;
            }
            Employee employee;
            try
            {
                employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
            } catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            var sectionsOfThisProject = await _context.ProjectSection
                .Where(s => s.ProjectId == id)
                .Where( s => s.CompanyId == employee.CompanyId)
                .Select(s => s.ProjectSectionId)
                .ToListAsync();
            TotalTasks = await _context.ProjectTask
                .Where(pt => sectionsOfThisProject.Contains(pt.ProjectSectionId))
                .Where(pt => pt.CompanyId == employee.CompanyId)
                .CountAsync();
            var completedState = await _context.State.FirstOrDefaultAsync(s => s.StateName == "Abgeschlossen");

            CompletetedTasks = await _context.ProjectTask
                .Where(pt => sectionsOfThisProject.Contains(pt.ProjectSectionId))
                .Where(pt => pt.CompanyId == employee.CompanyId)
                .Where(pt => pt.StateId == completedState.StateId)
                .CountAsync();
            try
            {
                throw new Exception("Test");
            }catch (Exception ex){
                await new Logger(_context, _userManager).Log(ex, User, Project);
            }


            return Page();
        }
    }
}
