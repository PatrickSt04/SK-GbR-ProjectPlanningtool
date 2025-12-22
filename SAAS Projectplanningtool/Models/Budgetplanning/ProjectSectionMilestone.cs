using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class ProjectSectionMilestone
    {
        [Key]
        [NotNull] public string ProjectSectionMilestoneId { get; set; } = Guid.NewGuid().ToString();
        public string? MilestoneName { get; set; }

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
        public DateOnly? Date { get; set; }

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
