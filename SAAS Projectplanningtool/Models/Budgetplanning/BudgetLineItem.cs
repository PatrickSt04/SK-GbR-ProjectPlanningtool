using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SAAS_Projectplanningtool.Models.ArticleManagement;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class BudgetLineItem
    {
        [Key]
        [NotNull] public string BudgetLineItemId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // BudgetGroup dependency
        public string BudgetGroupId { get; set; } = default!;
        [ForeignKey(nameof(BudgetGroupId))]
        public BudgetGroup? BudgetGroup { get; set; }

        public int SortOrder { get; set; }

        public BudgetLineItemType LineItemType { get; set; }

        // Material reference (only when LineItemType == Material)
        public string? ArticleId { get; set; }
        [ForeignKey(nameof(ArticleId))]
        public Article? Article { get; set; }

        // Resource reference (only when LineItemType == Resource)
        public string? ProjectHourlyRateGroupId { get; set; }
        [ForeignKey(nameof(ProjectHourlyRateGroupId))]
        public ProjectHourlyRateGroup? ProjectHourlyRateGroup { get; set; }

        // Description (article number/name, HRG name, or free text)
        public string Description { get; set; } = "";

        // Quantity (decimal)
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        // Unit price (from Article.Price, PHRG.ProjectHourlyRate, or manual)
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // Calculated price: Quantity × UnitPrice (not stored in DB)
        [NotMapped]
        public decimal CalculatedPrice => Quantity * UnitPrice;

        // Adjusted price: overrides calculated price (null = use calculated)
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AdjustedPrice { get; set; }

        // Effective price: AdjustedPrice if set, otherwise CalculatedPrice
        [NotMapped]
        public decimal EffectivePrice => AdjustedPrice ?? CalculatedPrice;

        // Latest Modification Attributes
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModifier { get; set; }

        public DateTime? LatestModificationTimestamp { get; set; }
        public string? LatestModificationText { get; set; }

        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Employee? CreatedByEmployee { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
    }
}
