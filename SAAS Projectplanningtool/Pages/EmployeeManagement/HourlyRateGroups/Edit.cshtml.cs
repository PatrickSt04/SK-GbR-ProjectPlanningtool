using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.HourlyRateGroups
{
    public class EditModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;

        public EditModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HourlyRateGroup HourlyRateGroup { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hourlyrategroup =  await _context.HourlyRateGroup.FirstOrDefaultAsync(m => m.HourlyRateGroupId == id);
            if (hourlyrategroup == null)
            {
                return NotFound();
            }
            HourlyRateGroup = hourlyrategroup;
           ViewData["CompanyId"] = new SelectList(_context.Company, "CompanyId", "CompanyId");
           ViewData["LatestModifierId"] = new SelectList(_context.Employee, "EmployeeId", "EmployeeId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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

            return RedirectToPage("./Index");
        }

        private bool HourlyRateGroupExists(string id)
        {
            return _context.HourlyRateGroup.Any(e => e.HourlyRateGroupId == id);
        }
    }
}
