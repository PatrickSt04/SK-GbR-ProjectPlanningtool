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

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement
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
            _logger = new Logger(_context, userManager);
        }

        public IList<Employee> Employee { get; set; } = default!;
        public IList<HourlyRateGroup> HourlyRateGroup { get; set; } = default!;

        public async Task OnGetAsync()
        {
            await _logger.Log(null, User, null, "Employees/Index<OnGet>Beginn");
            try
            {
                var employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                Employee = await _context.Employee
                .Include(e => e.Company)
                .Include(e => e.HourlyRateGroup)
                .Include(e => e.IdentityRole)
                .Include(e => e.IdentityUser)
                .Include(e => e.LatestModifier)
                .Where(e => e.CompanyId == employee.CompanyId)
                .ToListAsync();

                HourlyRateGroup = await _context.HourlyRateGroup
                    .Include(e => e.Company)
                    .Where(e => e.CompanyId == employee.CompanyId)
                    .ToListAsync();
                await _logger.Log(null, User, null, "Employees/Index<OnGet>End");
            }
            catch (Exception ex)
            {
                await _logger.Log(ex, User, Employee, "ERROR:Employees/Index<OnGet>End");
                Employee = new List<Employee>();
            }

        }
    }
}
