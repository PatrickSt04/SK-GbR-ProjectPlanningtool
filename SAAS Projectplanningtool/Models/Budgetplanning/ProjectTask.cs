using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class ProjectTask
    {
        [Key]
        [NotNull] public string ProjectTaskId { get; set; } = Guid.NewGuid().ToString();
        public string? ProjectTaskName { get; set; }
         
        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Reference to ProjectSection
        public string? ProjectSectionId { get; set; }
        [ForeignKey(nameof(ProjectSectionId))]
        public ProjectSection? ProjectSection { get; set; }

        // Amount of Workers per hourly rate group
        // p. Ex: < 5, Polier > 
        [NotMapped]
        public Dictionary<float, string>? AmountPerHourlyRateGroup { get; set; }

        public string? StateId { get; set; }
        [ForeignKey(nameof(StateId))]
        public SAAS_Projectplanningtool.Models.IndependentTables.State? State { get; set; }

        // Latest Modifier of Database Entry
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModidier { get; set; }

    }
}
