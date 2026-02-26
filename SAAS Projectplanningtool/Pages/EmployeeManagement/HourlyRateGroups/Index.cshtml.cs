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
    public class IndexModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context) : PageModel
    {
        public IList<HourlyRateGroup> HourlyRateGroup { get;set; } = default!;

        public async Task OnGetAsync()
        {
            HourlyRateGroup = await context.HourlyRateGroup
                .Include(h => h.Company)
                .Include(h => h.LatestModifier).ToListAsync();
        }
    }
}
