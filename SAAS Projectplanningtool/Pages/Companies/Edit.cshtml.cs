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

namespace SAAS_Projectplanningtool.Pages.Companies
{
    public class EditModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public EditModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
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
            Address = company.Address;

            if (_dayMap != null)
            {
                foreach (var day in _dayMap)
                {
                    SelectedWorkDays[day.Key] = Company.DefaultWorkDays.Contains(day.Key);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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
                existingCompany.Address.Country = Address.Country;
                existingCompany.Address.Region = Address.Region;
                existingCompany.Address.PostalCode = Address.PostalCode;
                existingCompany.Address.City = Address.City;
                existingCompany.Address.Street = Address.Street;
                existingCompany.Address.HouseNumber = Address.HouseNumber;
                existingCompany.Address.LatestModidier = await new CustomUserManager(_context, _userManager)
                    .GetEmployeeAsync(_userManager.GetUserId(User));

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

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(Company.CompanyId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Index");
        }

        private bool CompanyExists(string id)
        {
            return _context.Company.Any(e => e.CompanyId == id);
        }
    }
}
