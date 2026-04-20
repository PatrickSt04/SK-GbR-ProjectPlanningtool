using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SAAS_Projectplanningtool.Models.IndependentTables;

namespace SAAS_Projectplanningtool.Models.ArticleManagement

{
    public class Article
    {
        [Key] public string ArticleId { get; set; } = Guid.NewGuid().ToString();

        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [Required]
        public required string ArticleNumber { get; set; }

        [Required]
        public required string ArticleName { get; set; }

        public string? Description { get; set; }

        // Category FK
        public string? ArticleCategoryId { get; set; }
        [ForeignKey(nameof(ArticleCategoryId))]
        public ArticleCategory? ArticleCategory { get; set; }

        // Unit FK (Pflichtfeld)
        [Required]
        public string UnitId { get; set; } = default!;
        [ForeignKey(nameof(UnitId))]
        public Unit? Unit { get; set; }

        public bool DeleteFlag { get; set; } = false;

        // Price History Navigation
        public ICollection<ArticlePriceHistory> PriceHistory { get; set; } = new List<ArticlePriceHistory>();

        // Modification tracking
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModifier { get; set; }
        public DateTime? LatestModificationTimestamp { get; set; }
        public string? LatestModificationText { get; set; }

        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Employee? CreatedByEmployee { get; set; }
        public DateTime? CreatedTimestamp { get; set; }

        // Helper: aktueller Preis aus Historie
        [NotMapped]
        public decimal CurrentPrice => PriceHistory
            .OrderByDescending(p => p.Timestamp)
            .FirstOrDefault()?.Price ?? 0;
    }
}
