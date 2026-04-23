using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class Offer
    {
        [Key]
        [NotNull] public string OfferId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Project dependency
        public string ProjectId { get; set; } = default!;
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        public string OfferName { get; set; } = "";

        public DateTime OfferDate { get; set; } = DateTime.Now;

        /// <summary>
        /// MwSt-Satz als Dezimal (z.B. 0.19 für 19%)
        /// </summary>
        [Column(TypeName = "decimal(5,4)")]
        public decimal TaxRate { get; set; } = 0.19m;

        // Navigation
        public ICollection<OfferGroup> OfferGroups { get; set; } = new List<OfferGroup>();

        // Computed totals (not mapped)
        [NotMapped]
        public decimal TotalNetto => OfferGroups?.Sum(g => g.TotalPrice) ?? 0;

        [NotMapped]
        public decimal TotalBrutto => TotalNetto * (1 + TaxRate);

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
