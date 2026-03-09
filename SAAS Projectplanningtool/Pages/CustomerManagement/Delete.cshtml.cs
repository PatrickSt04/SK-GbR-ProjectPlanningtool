using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.CRM;

namespace SAAS_Projectplanningtool.Pages.CustomerManagement
{
    public class DeleteModel(SAAS_Projectplanningtool.Data.ApplicationDbContext context) : PageModel
    {
        [BindProperty]
        public Customer Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await context.Customer.FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }
            else
            {
                Customer = customer;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await context.Customer.FindAsync(id);
            if (customer != null)
            {
                Customer = customer;
                context.Customer.Remove(Customer);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
