using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.HourlyRateGroups
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
        ViewData["CompanyId"] = new SelectList(_context.Company, "CompanyId", "CompanyId");
        ViewData["LatestModifierId"] = new SelectList(_context.Employee, "EmployeeId", "EmployeeId");
            return Page();
        }

        [BindProperty]
        public HourlyRateGroup HourlyRateGroup { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.HourlyRateGroup.Add(HourlyRateGroup);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
