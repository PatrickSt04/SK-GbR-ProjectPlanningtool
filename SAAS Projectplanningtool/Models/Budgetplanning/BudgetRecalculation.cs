using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class BudgetRecalculation
    {
        [Key]
        [NotNull] public string RecalculationId { get; set; } = Guid.NewGuid().ToString();
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public double NewBudget { get; set; }

        public DateTime RecalculationDateTime { get; set; }

        public string? RecalculatedById { get; set; }
        [ForeignKey(nameof(RecalculatedById))]
        public Employee? RecalculatedBy { get; set; }


    }
}
