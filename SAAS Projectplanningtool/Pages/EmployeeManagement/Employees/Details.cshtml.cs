using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.Employees
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

        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await _logger.Log(null, User, null, "/EmployeeManagement/Employees/Details<OnGetAsync>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employee
                    .Include(e => e.IdentityRole)
                    .Include(employee => employee.Company)
                    .Include(employee => employee.HourlyRateGroup)
                    .Include(employee => employee.CreatedByEmployee)
                    .Include(employee => employee.LatestModifier)
                    .Include(employee => employee.IdentityUser)
                    .FirstOrDefaultAsync(m => m.EmployeeId == id);
                if (employee == null)
                {
                    return NotFound();
                }
                else
                {
                    Employee = employee;
                }
                await _logger.Log(null, User, null, "/EmployeeManagement/Employees/Details<OnGetAsync>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }
    }
}
