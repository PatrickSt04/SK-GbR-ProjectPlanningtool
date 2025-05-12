using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.Employees
{
    public class EditModel : EmployeePageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            try
            {
                await _logger.Log(null, User, null, "/EmployeeManagement/Employees/Edit<OnGetAsync>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employee.FirstOrDefaultAsync(m => m.EmployeeId == id);
                if (employee == null)
                {
                    return NotFound();
                }
                Employee = employee;


                await PublishHourlyRateGroupsAsync();
                ViewData["IdentityRoleId"] = new SelectList(_context.Roles, "Id", "Name");
                await _logger.Log(null, User, null, "/EmployeeManagement/Employees/Edit<OnGetAsync>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Employee, null) });

            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var executingEmployee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                bool selfmodification = executingEmployee.EmployeeId == Employee.EmployeeId;
                var trackedEmployees = _context.ChangeTracker.Entries<Employee>().ToList();
                _context.Attach(executingEmployee).State = EntityState.Detached;
                if (!selfmodification)
                {

                    Employee = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Mitarbeiter geändert", Employee, false);
                    _context.Attach(Employee).State = EntityState.Modified;
                }
                else
                {
                    // Bei einer Selbstmodifikation darf Latestmodifier nicht = der EmployeeId sein, da sonst ein System Failure kommt aufgrund von doppeltem Tracking
                    // Deshalb verwenden wir nur Text und Timestamp
                    Employee.LatestModificationText = "Selbstmodifikation";
                    Employee.LatestModificationTimestamp = DateTime.Now;
                    _context.Attach(Employee).State = EntityState.Modified;
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(Employee.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToPage("./Details", new { id = Employee.EmployeeId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Employee, null) });
            }
        }

        private bool EmployeeExists(string id)
        {
            return _context.Employee.Any(e => e.EmployeeId == id);
        }
    }
}
