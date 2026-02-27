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

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.Employees
{
    public class DeleteModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : PageModel
    {
        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public IList<string> EmployeeRoles { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) return NotFound();

            var employee = await context.Employee
                .Include(e => e.Company)
                .Include(e => e.IdentityUser)
                .Include(e => e.HourlyRateGroup)
                .Include(e => e.LatestModifier)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null) return NotFound();

            Employee = employee;

            if (employee.IdentityUser != null)
                EmployeeRoles = await userManager.GetRolesAsync(employee.IdentityUser);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null) return NotFound();

            var employee = await context.Employee.FindAsync(id);
            if (employee != null)
            {
                Employee = employee;
                context.Employee.Remove(Employee);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
