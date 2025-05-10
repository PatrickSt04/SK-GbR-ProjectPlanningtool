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

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.HourlyRateGroups
{
    public class EditModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public EditModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        [BindProperty]
        public HourlyRateGroup HourlyRateGroup { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            try
            {
                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Edit<OnGetAsync>Begin");
                if (id == null)
                {
                    return NotFound();
                }

                var hourlyrategroup = await _context.HourlyRateGroup.FirstOrDefaultAsync(m => m.HourlyRateGroupId == id);
                if (hourlyrategroup == null)
                {
                    return NotFound();
                }
                HourlyRateGroup = hourlyrategroup;
                await _logger.Log(null, User, null, "/EmployeeManagement/HourlyRateGroups/Edit<OnGetAsync>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
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
                HourlyRateGroup = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Stundensatzgruppe geändert", HourlyRateGroup, false);
                _context.Attach(HourlyRateGroup).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HourlyRateGroupExists(HourlyRateGroup.HourlyRateGroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToPage("./Details", new {id = HourlyRateGroup.HourlyRateGroupId});

            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, null) });
            }
        }

        private bool HourlyRateGroupExists(string id)
        {
            return _context.HourlyRateGroup.Any(e => e.HourlyRateGroupId == id);
        }
    }
}
