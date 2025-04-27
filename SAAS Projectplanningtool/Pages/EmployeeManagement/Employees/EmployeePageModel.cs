using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using System.Linq.Expressions;

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.Employees
{
    public class EmployeePageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        public EmployeePageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public SelectList? HourlyRateGroups { get; set; }
        public SelectList? IdentityRoles { get; set; }


        public async Task<Employee?> GetEmployeeAsync()
        {
            //here a exception can occur
            //therefore all exceptions are catched in the calling method
            var employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
            return employee;
        }
        public async Task PublishHourlyRateGroupsAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "EmployeeManagement/Employees/EmployeePageModel<PublishHourlyRateGroupsAsync>Beginn");

                var employee = await GetEmployeeAsync();
                var hourlyrategroups = await _context.HourlyRateGroup
                    .Where(c => c.CompanyId == employee.CompanyId) // Hier wird die CompanyId gefiltert
                    .Where(c => c.DeleteFlag == false) // Hier wird der DeleteFlag gefiltert
                    .OrderBy(c => c.HourlyRateGroupName)
                    .ToListAsync();

                HourlyRateGroups = new SelectList(hourlyrategroups, nameof(HourlyRateGroup.HourlyRateGroupId), nameof(HourlyRateGroup.HourlyRateGroupName));
                await _logger.Log(null, User, null, "EmployeeManagement/Employees/EmployeePageModel<PublishHourlyRateGroupsAsync>End");
            }
            catch (Exception ex)
            {
                await _logger.Log(ex, User, HourlyRateGroups, "EXCEPTION: EmployeeManagement/Employees/EmployeePageModel<PublishHourlyRateGroupsAsync>End");
                HourlyRateGroups = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            }
        }

        public async Task PublishRolesAsync(RoleManager<IdentityRole> _roleManager)
        {
            try
            {
                await _logger.Log(null, User, null, "EmployeeManagement/Employees/EmployeePageModel<PublishRolesAsync>Beginn");

                IdentityRoles = new SelectList(_roleManager.Roles, "Id", "Name");

                await _logger.Log(null, User, null, "EmployeeManagement/Employees/EmployeePageModel<PublishRolesAsync>End");
            }
            catch (Exception ex)
            {
                await _logger.Log(ex, User, HourlyRateGroups, "EXCEPTION: EmployeeManagement/Employees/EmployeePageModel<PublishRolesAsync>End");
                HourlyRateGroups = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            }
        }
    }
}
