using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SAAS_Projectplanningtool.Models.ArticleManagement;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class OfferLineItem
    {
        [Key]
        [NotNull] public string OfferLineItemId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // OfferGroup dependency
        public string OfferGroupId { get; set; } = default!;
        [ForeignKey(nameof(OfferGroupId))]
        public OfferGroup? OfferGroup { get; set; }

        public int SortOrder { get; set; }

        public BudgetLineItemType LineItemType { get; set; }

        // Material reference (snapshot)
        public string? ArticleId { get; set; }
        [ForeignKey(nameof(ArticleId))]
        public Article? Article { get; set; }

        // Resource reference (snapshot)
        public string? ProjectHourlyRateGroupId { get; set; }
        [ForeignKey(nameof(ProjectHourlyRateGroupId))]
        public ProjectHourlyRateGroup? ProjectHourlyRateGroup { get; set; }

        public string Description { get; set; } = "";

        /// <summary>
        /// Snapshot des Artikelnamens/Nummer zum Zeitpunkt der Angebotserstellung
        /// </summary>
        public string? SnapshotArticleName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Snapshot-Preis zum Zeitpunkt der Angebotserstellung (netto)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal CalculatedPrice => Quantity * UnitPrice;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AdjustedPrice { get; set; }

        [NotMapped]
        public decimal EffectivePrice => AdjustedPrice ?? CalculatedPrice;

        /// <summary>
        /// Einzelne Zeile ausblenden (ausgegraut anzeigen)
        /// </summary>
        public bool IsHidden { get; set; } = false;

        // Audit
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
