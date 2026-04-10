using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.IndependentTables;

namespace SAAS_Projectplanningtool.Pages.Settings.Units
{
    public class IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        : PageModel
    {
        private readonly Logger _logger = new(context, userManager);
        private readonly CustomUserManager _customUserManager = new(context, userManager);

        public IList<UnitViewModel> Units { get; set; } = [];
        public string CompanyName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "Settings/Units/Index<OnGet>Begin");

                var employee = await _customUserManager.GetEmployeeAsync(userManager.GetUserId(User));
                if (employee?.CompanyId == null) return NotFound();

                CompanyName = employee.Company?.CompanyName ?? string.Empty;

                var allUnits = await context.Unit
                    .OrderBy(u => u.Name)
                    .AsNoTracking()
                    .ToListAsync();

                var activeUnitIdList = await context.CompanyUnit
                    .Where(cu => cu.CompanyId == employee.CompanyId)
                    .Select(cu => cu.UnitId)
                    .ToListAsync();

                var activeUnitIds = new HashSet<string>(activeUnitIdList);

                Units = allUnits.Select(u => new UnitViewModel
                {
                    UnitId = u.UnitId,
                    Name = u.Name ?? string.Empty,
                    ShortName = u.ShortName,
                    IsActive = activeUnitIds.Contains(u.UnitId)
                }).ToList();

                await _logger.Log(null, User, null, "Settings/Units/Index<OnGet>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR:Settings/Units/Index<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostToggleAsync(string unitId)
        {
            try
            {
                await _logger.Log(null, User, null, "Settings/Units/Index<OnPostToggle>Begin");

                if (string.IsNullOrWhiteSpace(unitId)) return NotFound();

                var employee = await _customUserManager.GetEmployeeAsync(userManager.GetUserId(User));
                if (employee?.CompanyId == null) return NotFound();

                // Prüfen ob Unit existiert
                var unitExists = await context.Unit.AnyAsync(u => u.UnitId == unitId);
                if (!unitExists) return NotFound();

                var existing = await context.CompanyUnit
                    .FirstOrDefaultAsync(cu => cu.CompanyId == employee.CompanyId && cu.UnitId == unitId);

                if (existing != null)
                {
                    // Deaktivieren
                    context.CompanyUnit.Remove(existing);
                }
                else
                {
                    // Aktivieren
                    context.CompanyUnit.Add(new CompanyUnit
                    {
                        CompanyId = employee.CompanyId,
                        UnitId = unitId
                    });
                }

                await context.SaveChangesAsync();

                await _logger.Log(null, User, null, "Settings/Units/Index<OnPostToggle>End");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR:Settings/Units/Index<OnPostToggle>") });
            }
        }

        public class UnitViewModel
        {
            public string UnitId { get; set; } = default!;
            public string Name { get; set; } = default!;
            public string? ShortName { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
