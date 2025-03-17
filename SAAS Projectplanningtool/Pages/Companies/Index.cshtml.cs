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
    public class IndexModel : PageModel
    {
        private readonly SAAS_Projectplanningtool.Data.ApplicationDbContext _context;

        public IndexModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Company> Company { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Company = await _context.Company
                .Include(c => c.Address)
                .Include(c => c.License)
                .Include(c => c.Sector).ToListAsync();
        }
    }
}
