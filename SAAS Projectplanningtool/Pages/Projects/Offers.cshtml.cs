using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class OffersModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        : ProjectPageModel(context, userManager, roleManager)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly Logger _logger = new(context, userManager);

        public List<Offer> Offers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            await SetProjectBindingAsync(id);
            if (Project == null)
                return NotFound();

            var employee = await GetEmployeeAsync();
            if (employee == null || employee.CompanyId == null)
                return NotFound();

            Offers = await _context.Offer
                .Include(o => o.OfferGroups)
                    .ThenInclude(og => og.OfferLineItems)
                .Include(o => o.CreatedByEmployee)
                .Where(o => o.ProjectId == id && o.CompanyId == employee.CompanyId)
                .OrderByDescending(o => o.OfferDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string offerId, string projectId)
        {
            if (string.IsNullOrEmpty(offerId) || string.IsNullOrEmpty(projectId))
                return NotFound();

            try
            {
                var offer = await _context.Offer
                    .Include(o => o.OfferGroups)
                        .ThenInclude(og => og.OfferLineItems)
                    .FirstOrDefaultAsync(o => o.OfferId == offerId);

                if (offer != null)
                {
                    foreach (var group in offer.OfferGroups)
                    {
                        _context.OfferLineItem.RemoveRange(group.OfferLineItems);
                    }
                    _context.OfferGroup.RemoveRange(offer.OfferGroups);
                    _context.Offer.Remove(offer);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                await _logger.Log(ex, User, null, "Offers<OnPostDeleteAsync>Error");
            }

            return RedirectToPage("/Projects/Offers", new { id = projectId });
        }
    }
}
