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
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.Employees
{
    public class CreateModel : EmployeePageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Logger _logger;
        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
            _roleManager = roleManager;
        }
        [BindProperty]
        public Employee Employee { get; set; } = default!;
        [BindProperty]
        public string Email { get; set; } = string.Empty;
        public async Task<IActionResult> OnGet()
        {
            try
            {
                await _logger.Log(null, User, null, "/EmployeeManagement/Employees/Create<OnGet>Begin");
                //ViewData["HourlyRateGroupId"] = new SelectList(_context.HourlyRateGroup, "HourlyRateGroupId", "HourlyRateGroupName");
                //ViewData["IdentityRoleId"] = new SelectList(_context.Roles, "Id", "Name");
                await PublishHourlyRateGroupsAsync();
                await PublishRolesAsync(_roleManager);
                await _logger.Log(null, User, null, "/EmployeeManagement/Employees/Create<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
            //ViewData["CompanyId"] = new SelectList(_context.Company, "CompanyId", "CompanyId");

            //ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            //ViewData["LatestModifierId"] = new SelectList(_context.Employee, "EmployeeId", "EmployeeId");
        }



        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "/EmployeeManagement/Employees/Create<OnPostAsync>Begin");
                if (!ModelState.IsValid)
                {
                    await PublishHourlyRateGroupsAsync();
                    await PublishRolesAsync(_roleManager);
                    return Page();
                }

                var excecutingEmployee = await GetEmployeeAsync();

                var customUserManager = new CustomUserManager(_context, _userManager);
                // if identityuser couldnt be created, the exception is catched seperately, because then it will be not redirected to the error page
                // and the user will be redirected to the create page with the error message
                try
                {
                    var newIdentityUser = await customUserManager.CreateIdentityUser(Email, Employee.IdentityRoleId, _roleManager);
                    Employee.IdentityUserId = newIdentityUser.Id;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    await PublishHourlyRateGroupsAsync();
                    await PublishRolesAsync(_roleManager);
                    return Page();
                }


                Employee.CompanyId = excecutingEmployee.CompanyId;
                var customObjectModifier = new CustomObjectModifier(_context, _userManager);
                // Latest modifier & Created attributes are filled
                Employee = await customObjectModifier.AddLatestModificationAsync(User, "Benutzer wurde angelegt", Employee, true);



                _context.Employee.Add(Employee);
                await _context.SaveChangesAsync();
                await _logger.Log(null, User, null, "/EmployeeManagement/Employees/Create<OnPostAsync>End");
                return RedirectToPage("/EmployeeManagement/Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Employee, null) });
            }
        }
    }
}
