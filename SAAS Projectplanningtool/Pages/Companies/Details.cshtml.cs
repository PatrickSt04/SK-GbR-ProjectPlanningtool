using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.Companies
{
    public class DetailsModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;

        public DetailsModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Company Company { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }
            else
            {
                Company = company;
            }
            return Page();
        }
    }
}
