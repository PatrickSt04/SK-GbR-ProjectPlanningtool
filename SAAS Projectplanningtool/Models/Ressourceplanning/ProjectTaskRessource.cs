using SAAS_Projectplanningtool.Models.Budgetplanning;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.RessourcePlanning
{
    public class ProjectTaskRessource
    {
        [Key]
        [NotNull] public string ProjectTaskRessourceId { get; set; } = Guid.NewGuid().ToString();
        public string? ProjectTaskName { get; set; }

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Reference to the ProjectTask
        public string? ProjectTaskId { get; set; }
        [ForeignKey(nameof(ProjectTaskId))]
        public ProjectTask? ProjectTask { get; set; }

        // Amount of Workers per hourly rate group
        // p. Ex: < 5, Polier > 
        [NotMapped]
        public Dictionary<int, HourlyRateGroup>? AmountPerHourlyRateGroup { get; set; }

        //Employee Ressources
        public List<Employee>? EmployeeRessources { get; set; }

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
