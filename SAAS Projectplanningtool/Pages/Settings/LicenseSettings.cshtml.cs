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

namespace SAAS_Projectplanningtool.Pages.Settings
{
    public class LicenseSettingsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        : PageModel
    {
        private readonly Logger _logger = new(context, userManager);

        [BindProperty]
        public Company Company { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            await _logger.Log(null, User, null, "Companies/Edit<OnGet>Beginn");
            try
            {
                if (id == null)
                {
                    // Wenn keine ID gegeben wird, so wird die Company des angemeldeten Users verwendet
                    var loggedInEmployee = await new CustomUserManager(context, userManager).GetEmployeeAsync(userManager.GetUserId(User));
                    id = loggedInEmployee?.CompanyId;
                    if (id == null)
                    {
                        return NotFound();
                    }
                }

                var company = await context.Company
                    .Include(c => c.License)
                    .Include(c => c.Sector)
                    .Include(c => c.Address)
                    .AsNoTracking() // Verhindert das Tracking der Entität
                    .FirstOrDefaultAsync(m => m.CompanyId == id);

                if (company == null)
                {
                    return NotFound();
                }

                Company = company;
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Company, null) });
            }

            await _logger.Log(null, User, null, "Companies/Edit<OnGet>End");
            return Page();
        }
    }
}
