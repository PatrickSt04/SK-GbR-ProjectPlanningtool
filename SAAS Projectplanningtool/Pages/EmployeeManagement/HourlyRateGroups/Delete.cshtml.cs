using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.EmployeeManagement.HourlyRateGroups
{
    public class DeleteModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;

        public DeleteModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context)
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

            var hourlyrategroup = await _context.HourlyRateGroup.FirstOrDefaultAsync(m => m.HourlyRateGroupId == id);

            if (hourlyrategroup == null)
            {
                return NotFound();
            }
            else
            {
                HourlyRateGroup = hourlyrategroup;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hourlyrategroup = await _context.HourlyRateGroup.FindAsync(id);
            if (hourlyrategroup != null)
            {
                HourlyRateGroup = hourlyrategroup;
                _context.HourlyRateGroup.Remove(HourlyRateGroup);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
