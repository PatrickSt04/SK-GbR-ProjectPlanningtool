using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class BudgetGroup
    {
        [Key]
        [NotNull] public string BudgetGroupId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // ProjectBudget dependency
        public string ProjectBudgetId { get; set; } = default!;
        [ForeignKey(nameof(ProjectBudgetId))]
        public ProjectBudget? ProjectBudget { get; set; }

        [Required]
        public required string GroupName { get; set; }

        public int SortOrder { get; set; }

        // Navigation to line items
        public ICollection<BudgetLineItem> BudgetLineItems { get; set; } = new List<BudgetLineItem>();

        // Computed total (not mapped to DB)
        [NotMapped]
        public decimal TotalPrice => BudgetLineItems?.Sum(li => li.EffectivePrice) ?? 0;

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
