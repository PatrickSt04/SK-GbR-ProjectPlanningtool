using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class ProjectBudget
    {
        [Key]
        public required string ProjectBudgetId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        // The Initial Budget
        public float InitialBudget { get; set; }
        // The current Budget which is already used
        public float UsedBudget { get; set; }

        // Latest Modifier of Database Entry
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModidier { get; set; }
    }
}
