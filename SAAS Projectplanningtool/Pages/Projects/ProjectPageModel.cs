using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class ProjectPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logger _logger;

        public ProjectPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = new Logger(_context, _userManager);
        }

        public SelectList? Customers { get; set; }
        public SelectList? ProjectLeads { get; set; }

        // Lädt die SelectList mit allen Kunden (z.B. für Dropdown)
        public async Task PublishCustomersAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "ProjectPageModel<PublishCustomers>Beginn");
                var customers = await _context.Customer
                    .OrderBy(c => c.CustomerName)
                    .ToListAsync();

                Customers = new SelectList(customers, nameof(Customer.CustomerId), nameof(Customer.CustomerName));
                await _logger.Log(null, User, null, "ProjectPageModel<PublishCustomers>End");
            }
            catch (Exception ex)
            {
                await _logger.Log(ex, User, Customers, "EXCEPTION: ProjectPageModel<PublishCustomers>End");
                Customers = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            }
        }

        // Lädt die SelectList mit allen Benutzern mit der Rolle "Project Manager"
        public async Task PublishProjectLeadsAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "ProjectPageModel<PublishProjectLeadsAsync>Beginn");
                var plannerRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "Planner");

                if (plannerRole == null)
                {
                    // Rolle "Planner" existiert nicht
                    ProjectLeads = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
                    await _logger.Log(null, User, null, "ERROR: NO EMPLOYEE FOUND: ProjectPageModel<PublishProjectLeadsAsync>");
                    return;
                }
                var projectLeads = _context.Employee
                    .Where(e => e.IdentityRoleId == plannerRole.Id) // Hier wird die Rolle "Planner" gefiltert
                    .Select(u => new
                    {
                        Id = u.EmployeeId,
                        Name = u.EmployeeDisplayName // oder u.Email oder ein zusammengesetzter Anzeigename
                    })
                    .OrderBy(u => u.Name)
                    .ToList();

                ProjectLeads = new SelectList(projectLeads, "Id", "Name");
                await _logger.Log(null, User, null, "ProjectPageModel<PublishProjectLeadsAsync>End");
            }
            catch (Exception ex)
            {
                await _logger.Log(ex, User, ProjectLeads, "EXCEPTION: ProjectPageModel<PublishProjectLeadsAsync>End");
                ProjectLeads = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "Name");
            }
        }
    }
}
