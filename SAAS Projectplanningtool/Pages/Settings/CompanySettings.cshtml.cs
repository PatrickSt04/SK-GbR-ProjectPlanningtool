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
    public class EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        : PageModel
    {
        private readonly Logger _logger = new(context, userManager);


        [BindProperty]
        public Company Company { get; set; } = default!;

        [BindProperty]
        public Address Address { get; set; } = default!;


        [BindProperty]
        public DefaultWorkingTimeHandler WorkingTime { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            await _logger.Log(null, User, null, "Companies/Edit<OnGet>Beginn");
            if (id == null)
            {
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
                WorkingTime.LoadFromWorkingDays(company.DefaultWorkDays);

                // Load working hours
                WorkingTime.LoadFromWorkingHours(company.DefaultWorkingHours);
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
            if (!WorkingTime.IsValid())
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
                var existingCompany = await context.Company
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
                existingCompany.DefaultWorkDays = WorkingTime.GetSelectedWorkingDays();

                // Update working hours
                existingCompany.DefaultWorkingHours = WorkingTime.GetWorkingHours();

                existingCompany.CompanyName = Company.CompanyName;
                existingCompany.SectorId = Company.SectorId;
                existingCompany.LicenseId = Company.LicenseId;

                try
                {
                    existingCompany = await new CustomObjectModifier(context, userManager).AddLatestModificationAsync(User, "Unternehmensdaten wurden bearbeitet", existingCompany, false);
                }
                catch (Exception ex)
                {
                    return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Company, null) });
                }

                await context.SaveChangesAsync();
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