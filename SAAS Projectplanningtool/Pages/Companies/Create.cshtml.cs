using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.Companies
{
    public class CreateModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;

        public CreateModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["AddressId"] = new SelectList(_context.Address, "AddressId", "AddressId");
        ViewData["LicenseId"] = new SelectList(_context.LicenseModel, "LicenseId", "LicenseId");
        ViewData["SectorId"] = new SelectList(_context.IndustrySector, "SectorId", "SectorId");
            return Page();
        }

        [BindProperty]
        public Company Company { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Company.Add(Company);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
