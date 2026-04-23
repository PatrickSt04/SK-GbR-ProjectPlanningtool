using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.CustomManagers;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using System.Text.Json;

namespace SAAS_Projectplanningtool.Pages.Projects
{
    public class OfferEditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        : BudgetPlannerPageModel(context, userManager, roleManager)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly Logger _logger = new(context, userManager);

        public Offer? Offer { get; set; }
        public bool IsNew { get; set; }

        // DTOs for JSON communication
        public class OfferLineItemDto
        {
            public string? OfferLineItemId { get; set; }
            public string? GroupId { get; set; }
            public int LineItemType { get; set; }
            public string? ArticleId { get; set; }
            public string? ProjectHourlyRateGroupId { get; set; }
            public string Description { get; set; } = "";
            public string? SnapshotArticleName { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal? AdjustedPrice { get; set; }
            public int SortOrder { get; set; }
            public bool IsHidden { get; set; }
        }

        public class OfferGroupDto
        {
            public string? OfferGroupId { get; set; }
            public string GroupName { get; set; } = "";
            public int SortOrder { get; set; }
            public bool IsHidden { get; set; }
            public List<OfferLineItemDto> LineItems { get; set; } = new();
        }

        public List<OfferGroupDto> ExistingOfferGroups { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string? offerId, string? projectId)
        {
            try
            {
                // Edit existing offer
                if (!string.IsNullOrEmpty(offerId))
                {
                    Offer = await _context.Offer
                        .Include(o => o.Project)
                        .Include(o => o.OfferGroups)
                            .ThenInclude(og => og.OfferLineItems)
                        .FirstOrDefaultAsync(o => o.OfferId == offerId);

                    if (Offer == null) return NotFound();

                    await SetProjectBindingAsync(Offer.ProjectId);
                    IsNew = false;

                    ExistingOfferGroups = Offer.OfferGroups
                        .OrderBy(g => g.SortOrder)
                        .Select(g => new OfferGroupDto
                        {
                            OfferGroupId = g.OfferGroupId,
                            GroupName = g.GroupName,
                            SortOrder = g.SortOrder,
                            IsHidden = g.IsHidden,
                            LineItems = g.OfferLineItems.OrderBy(li => li.SortOrder).Select(li => new OfferLineItemDto
                            {
                                OfferLineItemId = li.OfferLineItemId,
                                GroupId = li.OfferGroupId,
                                LineItemType = (int)li.LineItemType,
                                ArticleId = li.ArticleId,
                                ProjectHourlyRateGroupId = li.ProjectHourlyRateGroupId,
                                Description = li.Description,
                                SnapshotArticleName = li.SnapshotArticleName,
                                Quantity = li.Quantity,
                                UnitPrice = li.UnitPrice,
                                AdjustedPrice = li.AdjustedPrice,
                                SortOrder = li.SortOrder,
                                IsHidden = li.IsHidden
                            }).ToList()
                        }).ToList();
                }
                // Create new offer from budget
                else if (!string.IsNullOrEmpty(projectId))
                {
                    await SetProjectBindingAsync(projectId);
                    if (Project == null) return NotFound();

                    IsNew = true;
                    Offer = new Offer
                    {
                        ProjectId = projectId,
                        OfferName = $"Angebot {DateTime.Now:dd.MM.yyyy}",
                        OfferDate = DateTime.Now
                    };

                    // Copy budget groups/lines as offer groups/lines (snapshot prices)
                    if (Project.ProjectBudgetId != null)
                    {
                        var employee = await GetEmployeeAsync();
                        var budgetGroups = await _context.BudgetGroup
                            .Include(bg => bg.BudgetLineItems)
                                .ThenInclude(li => li.Article)
                            .Include(bg => bg.BudgetLineItems)
                                .ThenInclude(li => li.ProjectHourlyRateGroup)
                                    .ThenInclude(phrg => phrg!.HourlyRateGroup)
                            .Where(bg => bg.ProjectBudgetId == Project.ProjectBudgetId)
                            .Where(bg => bg.CompanyId == employee!.CompanyId)
                            .OrderBy(bg => bg.SortOrder)
                            .ToListAsync();

                        ExistingOfferGroups = budgetGroups.Select(bg => new OfferGroupDto
                        {
                            GroupName = bg.GroupName,
                            SortOrder = bg.SortOrder,
                            LineItems = bg.BudgetLineItems.OrderBy(li => li.SortOrder).Select(li => new OfferLineItemDto
                            {
                                LineItemType = (int)li.LineItemType,
                                ArticleId = li.ArticleId,
                                ProjectHourlyRateGroupId = li.ProjectHourlyRateGroupId,
                                Description = li.Description,
                                SnapshotArticleName = li.Article != null ? $"{li.Article.ArticleNumber} - {li.Article.ArticleName}" : null,
                                Quantity = li.Quantity,
                                UnitPrice = li.UnitPrice,
                                AdjustedPrice = li.AdjustedPrice,
                                SortOrder = li.SortOrder
                            }).ToList()
                        }).ToList();
                    }
                }
                else
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", await _logger.Log(ex, User, null, "OfferEdit<OnGetAsync>Error"));
            }
        }

        // AJAX: Save offer
        public async Task<IActionResult> OnPostSaveOfferAsync(string? projectId, string? offerId, string? offerName)
        {
            try
            {
                var employee = await GetEmployeeAsync();
                if (employee == null || employee.CompanyId == null)
                    return new JsonResult(new { success = false, message = "Nicht autorisiert" });

                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var offerGroups = JsonSerializer.Deserialize<List<OfferGroupDto>>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (offerGroups == null)
                    return new JsonResult(new { success = false, message = "Ungültige Daten" });

                Offer offer;

                if (!string.IsNullOrEmpty(offerId))
                {
                    // Update existing offer
                    offer = await _context.Offer
                        .Include(o => o.OfferGroups)
                            .ThenInclude(og => og.OfferLineItems)
                        .FirstOrDefaultAsync(o => o.OfferId == offerId)
                        ?? throw new Exception("Angebot nicht gefunden");

                    // Delete existing groups/items
                    foreach (var group in offer.OfferGroups)
                    {
                        _context.OfferLineItem.RemoveRange(group.OfferLineItems);
                    }
                    _context.OfferGroup.RemoveRange(offer.OfferGroups);

                    if (!string.IsNullOrEmpty(offerName))
                        offer.OfferName = offerName;
                }
                else
                {
                    // Create new offer
                    offer = new Offer
                    {
                        CompanyId = employee.CompanyId,
                        ProjectId = projectId!,
                        OfferName = offerName ?? $"Angebot {DateTime.Now:dd.MM.yyyy}",
                        OfferDate = DateTime.Now
                    };
                    offer = await new CustomObjectModifier(_context, _userManager)
                        .AddLatestModificationAsync(User, "Angebot erstellt", offer, true);
                    _context.Offer.Add(offer);
                }

                // Create groups and line items
                foreach (var groupDto in offerGroups)
                {
                    if (string.IsNullOrWhiteSpace(groupDto.GroupName) && (groupDto.LineItems == null || groupDto.LineItems.Count == 0))
                        continue;

                    var newGroup = new OfferGroup
                    {
                        CompanyId = employee.CompanyId,
                        OfferId = offer.OfferId,
                        GroupName = groupDto.GroupName ?? "",
                        SortOrder = groupDto.SortOrder,
                        IsHidden = groupDto.IsHidden
                    };
                    newGroup = await new CustomObjectModifier(_context, _userManager)
                        .AddLatestModificationAsync(User, "Angebotsgruppe erstellt", newGroup, true);

                    foreach (var itemDto in groupDto.LineItems)
                    {
                        var newItem = new OfferLineItem
                        {
                            CompanyId = employee.CompanyId,
                            OfferGroupId = newGroup.OfferGroupId,
                            SortOrder = itemDto.SortOrder,
                            LineItemType = (BudgetLineItemType)itemDto.LineItemType,
                            ArticleId = itemDto.LineItemType == 0 ? itemDto.ArticleId : null,
                            ProjectHourlyRateGroupId = itemDto.LineItemType == 1 ? itemDto.ProjectHourlyRateGroupId : null,
                            Description = itemDto.Description,
                            SnapshotArticleName = itemDto.SnapshotArticleName,
                            Quantity = itemDto.Quantity,
                            UnitPrice = itemDto.UnitPrice,
                            AdjustedPrice = itemDto.AdjustedPrice,
                            IsHidden = itemDto.IsHidden
                        };
                        newItem = await new CustomObjectModifier(_context, _userManager)
                            .AddLatestModificationAsync(User, "Angebotsposition erstellt", newItem, true);
                        newGroup.OfferLineItems.Add(newItem);
                    }

                    _context.OfferGroup.Add(newGroup);
                }

                offer = await new CustomObjectModifier(_context, _userManager)
                    .AddLatestModificationAsync(User, "Angebot gespeichert", offer, string.IsNullOrEmpty(offerId));
                if (!string.IsNullOrEmpty(offerId))
                    _context.Offer.Update(offer);

                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, offerId = offer.OfferId, projectId = offer.ProjectId });
            }
            catch (Exception ex)
            {
                await _logger.Log(ex, User, null, "OfferEdit<OnPostSaveOfferAsync>Error");
                return new JsonResult(new { success = false, message = "Fehler beim Speichern" });
            }
        }

        // Reuse article search from BudgetPlanning
        public async Task<JsonResult> OnGetSearchArticles(string? search)
        {
            var employee = await GetEmployeeAsync();
            if (employee == null || employee.CompanyId == null)
                return new JsonResult(new List<object>());

            var query = _context.Article
                .Where(a => a.CompanyId == employee.CompanyId && !a.DeleteFlag);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(a =>
                    a.ArticleNumber.ToLower().Contains(searchLower) ||
                    a.ArticleName.ToLower().Contains(searchLower));
            }

            var articles = await query.OrderBy(a => a.ArticleNumber).Take(50)
                .Select(a => new
                {
                    articleId = a.ArticleId,
                    articleNumber = a.ArticleNumber,
                    articleName = a.ArticleName,
                    price = a.CurrentPrice
                }).ToListAsync();

            return new JsonResult(articles);
        }

        public async Task<JsonResult> OnGetHourlyRateGroupsJson(string? projectId)
        {
            var employee = await GetEmployeeAsync();
            if (employee == null || employee.CompanyId == null)
                return new JsonResult(new List<object>());

            var query = _context.ProjectHourlyRateGroup
                .Include(h => h.HourlyRateGroup)
                .Where(p => p.HourlyRateGroup != null && !p.HourlyRateGroup.DeleteFlag)
                .Where(p => p.CompanyId == employee.CompanyId);

            if (!string.IsNullOrWhiteSpace(projectId))
                query = query.Where(p => p.ProjectId == projectId);

            var groups = await query.Select(p => new
            {
                projectHourlyRateGroupId = p.ProjectHourlyRateGroupId,
                name = p.HourlyRateGroup != null ? p.HourlyRateGroup.HourlyRateGroupName : "Unbekannt",
                hourlyRate = p.ProjectHourlyRate ?? (p.HourlyRateGroup != null ? p.HourlyRateGroup.HourlyRate : 0)
            }).ToListAsync();

            return new JsonResult(groups);
        }
    }
}
