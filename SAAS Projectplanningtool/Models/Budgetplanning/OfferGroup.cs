using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class OfferGroup
    {
        [Key]
        [NotNull] public string OfferGroupId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Offer dependency
        public string OfferId { get; set; } = default!;
        [ForeignKey(nameof(OfferId))]
        public Offer? Offer { get; set; }

        public string GroupName { get; set; } = "";

        public int SortOrder { get; set; }

        /// <summary>
        /// Wenn true, werden alle Zeilen dieser Gruppe im Angebot ausgeblendet (ausgegraut)
        /// </summary>
        public bool IsHidden { get; set; } = false;

        // Navigation to line items
        public ICollection<OfferLineItem> OfferLineItems { get; set; } = new List<OfferLineItem>();

        [NotMapped]
        public decimal TotalPrice => OfferLineItems?
            .Where(li => !li.IsHidden)
            .Sum(li => li.EffectivePrice) ?? 0;

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
