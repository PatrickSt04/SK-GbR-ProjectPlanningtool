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

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.HourlyRateGroups
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public CreateModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Create<OnGet>Begin");
                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Create<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
            //ViewData["CompanyId"] = new SelectList(_context.Company, "CompanyId", "CompanyId");
            //ViewData["LatestModifierId"] = new SelectList(_context.Employee, "EmployeeId", "EmployeeId");
        }

        [BindProperty]
        public HourlyRateGroup HourlyRateGroup { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _logger.Log(null, User, HourlyRateGroup, "/EmployeeManagement/HourlyRateGroups/Create<OnPostAsync>Begin");
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                var excecutingEmployee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                HourlyRateGroup.Company = excecutingEmployee.Company;

                HourlyRateGroup = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Stundensatzgruppe angelegt", HourlyRateGroup, true);

                _context.HourlyRateGroup.Add(HourlyRateGroup);
                await _context.SaveChangesAsync();

                await _logger.Log(null, User, HourlyRateGroup, "/EmployeeManagement/HourlyRateGroups/Create<OnPostAsync>End");
                return RedirectToPage("/EmployeeManagement/Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, HourlyRateGroup, null) });
            }
        }
    }
}
