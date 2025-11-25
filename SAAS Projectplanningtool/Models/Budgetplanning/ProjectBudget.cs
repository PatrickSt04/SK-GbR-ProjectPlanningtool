using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class ProjectBudget
    {
        [Key]
        [NotNull] public string ProjectBudgetId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        // The Initial Budget
        public double InitialBudget { get; set; } = 0.0d;

        public List<BudgetRecalculation> BudgetRecalculations { get; set; } = new();

        public List<InitialHRGPlanning> InitialHRGPlannings { get; set; } = new();

        public List<InitialAdditionalCost> InitialAdditionalCosts { get; set; } = new();

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

        public class InitialAdditionalCost
        {
            public string AdditionalCostName { get; set; } = "";
            public double AdditionalCostAmount { get; set; }
        }
        public class InitialHRGPlanning
        {
            public string HourlyRateGroupId { get; set; } = "";
            public string HourlyRateGroupName { get; set; } = "";
            public decimal HourlyRate { get; set; }
            public int Amount { get; set; }
            public double EstimatedHours { get; set; }
        }
    }
}
