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

        // Start Date of Project
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly? StartDate { get; set; }

        // End Date of Project
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly? EndDate { get; set; }

        public string? StateId { get; set; }
        [ForeignKey(nameof(StateId))]
        public SAAS_Projectplanningtool.Models.IndependentTables.State? State { get; set; }

        //Is relevant for task catalog
        public bool IsTaskCatalogEntry { get; set; } = true;

        //Is relevant for the task schedule
        public bool IsScheduleEntry { get; set; } = true;

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


        // Amount of Workers per hourly rate group
        // p. Ex: < 5, Polier > 
        public ICollection<ProjectTaskHourlyRateGroup> ProjectTaskHourlyRateGroups { get; set; } = new List<ProjectTaskHourlyRateGroup>();
    }
}
