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
using System.ComponentModel.DataAnnotations;

namespace SAAS_Projectplanningtool.Pages.Settings
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        public readonly DefaultWorkingTimeHandler _defaultWorkingTimeHandler;

        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(context, userManager);
            _defaultWorkingTimeHandler = new DefaultWorkingTimeHandler(context, userManager);
        }

        [BindProperty]
        public Company Company { get; set; } = default!;

        [BindProperty]
        public Address Address { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync(string? id)
        {
            await _logger.Log(null, User, null, "Companies/Edit<OnGet>Beginn");
            if (id == null)
            {
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
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

            Company = company;
            try
            {
                Address = company.Address;

                // Load working days checkboxes
                _defaultWorkingTimeHandler.LoadWorkingDaysToProperties(company.DefaultWorkDays);

                // Load working hours
                _defaultWorkingTimeHandler.LoadWorkingHoursToProperties(company.DefaultWorkingHours);
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

            // Validate working hours
            if (!_defaultWorkingTimeHandler.ValidateWorkingHours())
            {
                ModelState.AddModelError("", "Bitte stellen Sie sicher, dass die Endzeit nach der Startzeit liegt.");
                return Page();
            }

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

                // Update address
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

                // Update working days
                existingCompany.DefaultWorkDays = _defaultWorkingTimeHandler.GetSelectedWorkingDays();

                // Update working hours
                existingCompany.DefaultWorkingHours = _defaultWorkingTimeHandler.GetWorkingHoursFromProperties();

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