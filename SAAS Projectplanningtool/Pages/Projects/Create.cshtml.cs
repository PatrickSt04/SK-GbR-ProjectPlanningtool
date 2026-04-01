using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.Models.CRM;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class CreateModel : ProjectPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;
        public readonly DefaultWorkingTimeHandler _defaultWorkingTimeHandler;

        public Customer? customer;

        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
            : base(context, userManager, roleManager)

        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        //[BindProperty]
        //public float InitialBudget { get; set; } = 0.0f;

        [BindProperty]
        public DefaultWorkingTimeHandler WorkingTime { get; set; } = new();

        /// <summary>
        /// Projektspezifische Stundensätze – vorbelegt mit den Standardwerten der HourlyRateGroups.
        /// Können vom Benutzer bei der Projektanlage überschrieben werden.
        /// </summary>
        [BindProperty]
        public List<ProjectHourlyRateGroup> ProjectHourlyRateGroups { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string? customerid)
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/CreateModel<OnGetAsync>Beginn");

                if (customerid != null)
                    customer = await getCustomer(customerid);
                else
                    await PublishCustomersAsync();

                await PublishCustomersAsync();
                await PublishProjectLeadsAsync();
                await LoadCompanyDefaultWorkingTimesAsync();

                // ✅ NEU: Stundensatzgruppen laden und als Projektstundensätze vorbelegen
                await LoadProjectHourlyRateGroupsAsync();

                await _logger.Log(null, User, null, "Projects/CreateModel<OnGetAsync>End");
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Project, null) });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "Projects/CreateModel<OnPostAsync>Beginn");

                if (!ModelState.IsValid || !WorkingTime.IsValid())
                {
                    await PublishCustomersAsync();
                    await PublishProjectLeadsAsync();
                    await LoadCompanyDefaultWorkingTimesAsync();
                    await LoadProjectHourlyRateGroupsAsync();
                    return Page();
                }

                var companyUser = await GetCurrentEmployeeAsync();

                var projectBudget = await CreateProjectBudgetAsync(companyUser.CompanyId);

                Project.ProjectBudget = projectBudget;
                Project.CompanyId = companyUser.CompanyId;
                Project.DefaultWorkDays = WorkingTime.GetSelectedWorkingDays();
                Project.DefaultWorkingHours = WorkingTime.GetWorkingHours();

                Project = await new CustomObjectModifier(_context, _userManager)
                    .AddLatestModificationAsync(User, "Projekt angelegt", Project, true);

                _context.Project.Add(Project);
                await _context.SaveChangesAsync();

                // ✅ Projektspezifische Stundensätze mit den (ggf. angepassten) Werten speichern
                await CreateProjectHourlyRateGroupsAsync(companyUser.CompanyId);

                await _logger.Log(null, User, null, "Projects/CreateModel<OnPostAsync>End");
                return RedirectToPage("/Projects/BudgetPlanning", new { id = Project.ProjectId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Project, null) });
            }
        }
        public async Task<Customer>? getCustomer(string customerId)
        {
            var employee = await new CustomUserManager(_context, _userManager).GetEmployeeAsync(_userManager.GetUserId(User));
            return await _context.Customer.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.CompanyId == employee.CompanyId);
        }

        #region Private Helper Methods

        private async Task LoadCompanyDefaultWorkingTimesAsync()
        {
            var employee = await GetCurrentEmployeeAsync();
            if (string.IsNullOrEmpty(employee?.CompanyId)) return;

            var company = await _context.Company
                .FirstOrDefaultAsync(c => c.CompanyId == employee.CompanyId);

            if (company != null)
            {
                WorkingTime.LoadFromWorkingDays(company.DefaultWorkDays);
                WorkingTime.LoadFromWorkingHours(company.DefaultWorkingHours);
            }
        }

        private async Task<Employee> GetCurrentEmployeeAsync()
        {
            var userId = _userManager.GetUserId(User);
            return await new CustomUserManager(_context, _userManager).GetEmployeeAsync(userId);
        }

        private async Task<ProjectBudget> CreateProjectBudgetAsync(string companyId)
        {
            var projectBudget = new ProjectBudget
            {
                //InitialBudget = InitialBudget,
                CompanyId = companyId,
            };

            return await new CustomObjectModifier(_context, _userManager)
                .AddLatestModificationAsync(User, "Projektbudget angelegt", projectBudget, true);
        }

        #endregion
        #region Private Helper Methods

        /// <summary>
        /// Lädt alle aktiven HourlyRateGroups des Unternehmens und befüllt
        /// ProjectHourlyRateGroups mit den jeweiligen Standardstundensätzen als Vorschlagswert.
        /// </summary>
        private async Task LoadProjectHourlyRateGroupsAsync()
        {
            var employee = await GetCurrentEmployeeAsync();
            if (string.IsNullOrEmpty(employee?.CompanyId)) return;

            var hourlyRateGroups = await _context.HourlyRateGroup
                .Where(hrg => hrg.CompanyId == employee.CompanyId && !hrg.DeleteFlag)
                .OrderBy(hrg => hrg.HourlyRateGroupName)
                .ToListAsync();

            ProjectHourlyRateGroups = hourlyRateGroups.Select(hrg => new ProjectHourlyRateGroup
            {
                HourlyRateGroupId = hrg.HourlyRateGroupId,
                HourlyRateGroup = hrg,                          // Für die Anzeige im View
                ProjectHourlyRate = (decimal)hrg.HourlyRate,      // Standardwert vorbelegen
                CompanyId = employee.CompanyId
            }).ToList();
        }

        /// <summary>
        /// Speichert für jede HourlyRateGroup einen projektspezifischen Stundensatz.
        /// Verwendet dabei die vom Benutzer eingetragenen (ggf. angepassten) Werte
        /// aus der gebundenen Liste <see cref="ProjectHourlyRateGroups"/>.
        /// </summary>
        private async Task CreateProjectHourlyRateGroupsAsync(string companyId)
        {
            if (!ProjectHourlyRateGroups.Any()) return;

            var modifier = new CustomObjectModifier(_context, _userManager);

            foreach (var entry in ProjectHourlyRateGroups)
            {
                entry.CompanyId = companyId;
                entry.ProjectId = Project.ProjectId;

                var audited = await modifier.AddLatestModificationAsync(
                    User,
                    "Projektspezifischer Stundensatz bei Projektanlage erstellt",
                    entry,
                    true
                );

                _context.ProjectHourlyRateGroup.Add(audited);
            }

            await _context.SaveChangesAsync();
        }

        // ... bestehende Hilfsmethoden bleiben unverändert

        #endregion
    }
}