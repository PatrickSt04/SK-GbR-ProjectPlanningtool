using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class AdditionalCostsDetailModel : ProjectPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdditionalCostsDetailModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
            : base(context, userManager, roleManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<CostEntry> CostEntries { get; set; } = new();
        public double TotalCosts { get; set; }

        public class CostEntry
        {
            public string CostId { get; set; } = string.Empty;
            public string CostName { get; set; } = string.Empty;
            public double Amount { get; set; }
            public DateTime? CreatedTimestamp { get; set; }
            public string? CreatedBy { get; set; }
            public DateTime? ModifiedTimestamp { get; set; }
            public string? ModifiedBy { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                    return NotFound();

                await SetProjectBindingAsync(id);
                if (Project == null)
                    return NotFound();

                var employee = await GetEmployeeAsync();
                if (employee?.CompanyId == null)
                    return NotFound();

                var costs = await _context.ProjectAdditionalCosts
                    .Include(c => c.LatestModifier)
                    .Include(c => c.CreatedByEmployee)
                    .Where(c => c.ProjectId == id)
                    .Where(c => c.CompanyId == employee.CompanyId)
                    .OrderByDescending(c => c.CreatedTimestamp)
                    .ToListAsync();

                CostEntries = costs.Select(c => new CostEntry
                {
                    CostId = c.ProjectAdditionalCostsId,
                    CostName = c.AdditionalCostName ?? "Unbenannt",
                    Amount = c.AdditionalCostAmount ?? 0,
                    CreatedTimestamp = c.CreatedTimestamp,
                    CreatedBy = c.CreatedByEmployee?.EmployeeDisplayName,
                    ModifiedTimestamp = c.LatestModificationTimestamp,
                    ModifiedBy = c.LatestModifier?.EmployeeDisplayName
                }).ToList();

                TotalCosts = CostEntries.Sum(c => c.Amount);

                return Page();
            }
            catch (Exception ex)
            {
                //TODO : Log error
            }
            return Page();
        }
    }
}