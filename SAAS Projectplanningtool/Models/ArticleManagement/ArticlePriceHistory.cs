using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.ArticleManagement
{
    public class ArticlePriceHistory
    {
        [Key] public string ArticlePriceHistoryId { get; set; } = Guid.NewGuid().ToString();

        // Article FK
        [Required]
        public string ArticleId { get; set; } = default!;
        [ForeignKey(nameof(ArticleId))]
        public Article? Article { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string? Comment { get; set; }

        // Employee who created this entry
        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Employee? CreatedByEmployee { get; set; }
    }
}
