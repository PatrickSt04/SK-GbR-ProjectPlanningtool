using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.CRM;

namespace SAAS_Projectplanningtool.Pages.CustomerManagement
{
    public class CreateModel(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager) : PageModel
    {
        private readonly Logger _logger = new(context, userManager);

        [BindProperty] public Customer Customer { get; set; } = default!;
        [BindProperty] public Address Address { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _logger.Log(null, User, null, "CustomerManagement/Create<OnGet>Begin");
                var employee = await new CustomUserManager(context, userManager)
                    .GetEmployeeAsync(userManager.GetUserId(User));

                Address = new Address { CompanyId = employee.CompanyId };
                Customer = new Customer
                {
                    CustomerName = "",
                    CompanyId = employee.CompanyId,
                    CustomerType = CustomerType.Interessent
                };
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, null, "ERROR: CustomerManagement/Create<OnGet>") });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _logger.Log(null, User, Address, "CustomerManagement/Create<OnPost>Begin");
                if (!ModelState.IsValid) return Page();

                context.Address.Add(Address);
                await context.SaveChangesAsync();

                Customer = await new CustomObjectModifier(context, userManager)
                    .AddLatestModificationAsync(User, "Eintrag angelegt", Customer, true);
                Customer.Address = Address;

                context.Customer.Add(Customer);
                await context.SaveChangesAsync();

                await _logger.Log(null, User, Customer, "CustomerManagement/Create<OnPost>End");
                return RedirectToPage("./Details", new { id = Customer.CustomerId });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error",
                    new { id = await _logger.Log(ex, User, Customer, "ERROR: CustomerManagement/Create<OnPost>") });
            }
        }
    }
}
