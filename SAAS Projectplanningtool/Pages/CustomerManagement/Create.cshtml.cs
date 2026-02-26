using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.Pages.CustomerManagement
{
    public class CreateModel(
        SAAS_Projectplanningtool.Data.ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
        : PageModel
    {
        private readonly Logger _logger = new(context, userManager);

        [BindProperty]
        public Customer Customer { get; set; } = default!;

        [BindProperty]
        public Address Address { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync()
        {
            //Adresse erstellt, damit man die ID bereits hat
            try
            {
                await _logger.Log(null, User, null, "CustomerManagement/Create<OnGet>Begin");

                var employee = await new CustomUserManager(context, userManager).GetEmployeeAsync(userManager.GetUserId(User));
                Address = new Address { CompanyId = employee.CompanyId };
                Customer = new Customer { CustomerName = "", CompanyId = employee.CompanyId };

                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, null, "ERROR: CustomerManagement/Create<OnGet>") });
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _logger.Log(null, User, Address, "CustomerManagement/Create<OnPostAsync>Begin");
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                //Address wurde in der HTML Page gefüllt
                context.Address.Add(Address);
                await context.SaveChangesAsync();
                Customer = await new CustomObjectModifier(context, userManager).AddLatestModificationAsync(User, "Kunde wurde angelegt", Customer, true);
                Customer.Address = Address;
                context.Customer.Add(Customer);
                await context.SaveChangesAsync();

                await _logger.Log(null, User, Customer, "CustomerManagement/Create<OnPostAsync>End");

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { id = await _logger.Log(ex, User, Customer, "ERROR: CustomerManagement/Create<OnPost>") });
            }
        }
    }
}
