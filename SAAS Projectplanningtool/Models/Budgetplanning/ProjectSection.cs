using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class ProjectSection
    {
        [Key]
        [NotNull] public string ProjectSectionId { get; set; } = Guid.NewGuid().ToString();
        public required string ProjectSectionName { get; set; } 
        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Reference to the Project
        public string? ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        public string? SubSectionId { get; set; }
        [ForeignKey(nameof(SubSectionId))]
        public ProjectSection? SubSection { get; set; }
        // ParentSectionId to reference a parent ProjectSection
        public string? ParentSectionId { get; set; }
        [ForeignKey("ParentSectionId")]
        public ProjectSection? ParentSection { get; set; }

        // Collection to hold child sections
        public ICollection<ProjectSection>? SubSections { get; set; } = new List<ProjectSection>();

        public string? StateId { get; set; }
        [ForeignKey(nameof(StateId))]
        public SAAS_Projectplanningtool.Models.IndependentTables.State? State { get; set; }

        // Latest Modification Attributes
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModifier { get; set; }

        public DateTime? LatestModificationTimestamp { get; set; }
        public string? LatestModificationText { get; set; }
    }
}
