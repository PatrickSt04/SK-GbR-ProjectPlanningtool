using SAAS_Projectplanningtool.Models.ArticleManagement;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.ArticleManagement
{
    public class ArticleCategory
    {
        [Key] public string ArticleCategoryId { get; set; } = Guid.NewGuid().ToString();

        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [Required]
        public required string CategoryName { get; set; }

        public string? Description { get; set; }

        public bool DeleteFlag { get; set; } = false;

        // Navigation
        public ICollection<Article> Articles { get; set; } = new List<Article>();

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
    }
}
