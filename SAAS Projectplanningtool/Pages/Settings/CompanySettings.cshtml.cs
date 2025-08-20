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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(context, userManager);
        }

        [BindProperty]
        public Company Company { get; set; } = default!;
        [BindProperty]
        public Address Address { get; set; } = default!;

        public readonly Dictionary<int, string> _dayMap = new()
        {
            { 1, "Montag" },
            { 2, "Dienstag" },
            { 3, "Mittwoch" },
            { 4, "Donnerstag" },
            { 5, "Freitag" },
            { 6, "Samstag" },
            { 7, "Sonntag" }
        };

        [BindProperty]
        public Dictionary<int, bool> SelectedWorkDays { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            await _logger.Log(null, User, null, "Companies/Edit<OnGet>Beginn");
            if (id == null)
            {
                // Wenn keine ID gegeben wird, so wird die Company des angemeldeten Users verwendet
                var loggedInEmployee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
                id = loggedInEmployee?.CompanyId;
                if (id == null)
                {
                    return NotFound();
                }
            }

            var company = await _context.Company
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
            try
            {
                Address = company.Address;
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Address, null) });
            }
            try
            {
                if (_dayMap != null)
                {
                    foreach (var day in _dayMap)
                    {
                        SelectedWorkDays[day.Key] = Company.DefaultWorkDays.Contains(day.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Address, null) });
            }

            await _logger.Log(null, User, null, "Companies/Edit<OnGet>End");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _logger.Log(null, User, null, "Companies/Edit<OnPost>Beginn");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var existingCompany = await _context.Company
                    .Include(c => c.Address)
                    .FirstOrDefaultAsync(c => c.CompanyId == Company.CompanyId);

                if (existingCompany == null)
                {
                    return NotFound();
                }

                // Adresse aktualisieren
                try
                {
                    existingCompany.Address.Country = Address.Country;
                    existingCompany.Address.Region = Address.Region;
                    existingCompany.Address.PostalCode = Address.PostalCode;
                    existingCompany.Address.City = Address.City;
                    existingCompany.Address.Street = Address.Street;
                    existingCompany.Address.HouseNumber = Address.HouseNumber;
                }
                catch (Exception ex)
                {
                    return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, existingCompany, null) });
                }


                if (SelectedWorkDays != null)
                {
                    existingCompany.DefaultWorkDays = SelectedWorkDays
                        .Where(day => day.Value)
                        .Select(day => day.Key)
                        .ToList();
                }

                existingCompany.CompanyName = Company.CompanyName;
                existingCompany.SectorId = Company.SectorId;
                existingCompany.LicenseId = Company.LicenseId;

                try
                {
                    existingCompany = await new CustomObjectModifier(_context, _userManager).AddLatestModificationAsync(User, "Unternehmensdaten wurden bearbeitet", existingCompany, false);
                }
                catch (Exception ex)
                {
                    return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Company, null) });
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Company, null) });
            }
            await _logger.Log(null, User, null, "Companies/Edit<OnPost>End");
            return RedirectToPage("/Index");
        }
    }
}
