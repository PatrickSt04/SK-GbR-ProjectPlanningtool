using Mono.TextTemplating;
using SAAS_Projectplanningtool.Models.IndependentTables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class Project
    {
        [Key]
        [NotNull] public string ProjectId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Reference to Customer of this Project
        public   string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        // Reference to ProjectBudget of this Project
        public string? ProjectBudgetId { get; set; }
        [ForeignKey(nameof(ProjectBudgetId))]
        public ProjectBudget? ProjectBudget { get; set; }

        public required string ProjectName { get; set; }
        public required  string ProjectDescription { get; set; }

        // Start Date of Project
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly? StartDate { get; set; }

        // End Date of Project
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly? EndDate { get; set; }
        // Instead of Deleting a completed Project it is archived 
        public bool? IsArchived { get; set; }

        public string? StateId { get; set; }
        [ForeignKey(nameof(StateId))]
        public SAAS_Projectplanningtool.Models.IndependentTables.State? State { get; set; }
        // Latest Modifier of Database Entry
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModidier { get; set; }
    }
}
