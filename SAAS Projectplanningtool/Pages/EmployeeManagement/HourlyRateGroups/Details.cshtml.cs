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

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.HourlyRateGroups
{
    public class DetailsModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public DetailsModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public HourlyRateGroup HourlyRateGroup { get; set; } = default!;
        public bool isUsedInEmployee { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Details<OnGetAsync>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var hourlyrategroup = await _context.HourlyRateGroup
                    .Include(e => e.LatestModifier)
                    .Include(e => e.CreatedByEmployee)
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(m => m.HourlyRateGroupId == id);
                if (hourlyrategroup == null)
                {
                    return NotFound();
                }
                else
                {
                    HourlyRateGroup = hourlyrategroup;
                }

                var executingEmployee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                if (executingEmployee == null)
                {
                    return NotFound();
                }
                var employeesUsingHourlyRateGroup = await _context.Employee
                    .Include(e => e.HourlyRateGroup)
                    .Where(e => e.HourlyRateGroupId == id && e.CompanyId == executingEmployee.CompanyId)
                    .ToListAsync();
                isUsedInEmployee = employeesUsingHourlyRateGroup.Count > 0;

                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Details<OnGetAsync>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }
        public async Task<IActionResult> OnPostSetDeleteFlagAsync(string? id)
        {
            try
            {
                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Details<OnSetDeleteFlag>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var hourlyRateGroup = await _context.HourlyRateGroup.FirstOrDefaultAsync(e => e.HourlyRateGroupId == id);
                if (hourlyRateGroup == null)
                {
                    return NotFound();
                }

                hourlyRateGroup.DeleteFlag = true;
                hourlyRateGroup = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Löschkennzeichen gesetzt", hourlyRateGroup, false);
                _context.HourlyRateGroup.Update(hourlyRateGroup);
                await _context.SaveChangesAsync();

                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Details<OnSetDeleteFlag>End");
                return RedirectToPage("/EmployeeManagement/Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        public async Task<IActionResult> OnPostUndoDeleteFlagAsync(string? id)
        {
            try
            {
                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Details<OnUndoDeleteFlag>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var hourlyRateGroup = await _context.HourlyRateGroup.FirstOrDefaultAsync(e => e.HourlyRateGroupId == id);
                if (hourlyRateGroup == null)
                {
                    return NotFound();
                }

                hourlyRateGroup.DeleteFlag = false;
                hourlyRateGroup = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Löschkennzeichen gesetzt", hourlyRateGroup, false);

                _context.HourlyRateGroup.Update(hourlyRateGroup);
                await _context.SaveChangesAsync();

                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Details<OnUndoDeleteFlag>End");
                return RedirectToPage("/EmployeeManagement/Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string? id)
        {
            try
            {
                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Details<OnPostDeleteAsync>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var hourlyRateGroup = await _context.HourlyRateGroup.FirstOrDefaultAsync(e => e.HourlyRateGroupId == id);
                if (hourlyRateGroup == null)
                {
                    return NotFound();
                }

                _context.HourlyRateGroup.Remove(hourlyRateGroup);
                await _context.SaveChangesAsync();

                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Details<OnPostDeleteAsync>End");
                return RedirectToPage("/EmployeeManagement/Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }
    }
}
