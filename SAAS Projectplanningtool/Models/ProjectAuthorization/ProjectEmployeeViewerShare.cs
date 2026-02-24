using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models
{
    public class ProjectEmployeeViewerShare
    {
        //This class represents the sharing of a Project with an Employee which is a Viewer License. This Employee can only view Project that are Shared with him.
        [Key]
        [NotNull] public string ProjectEmployeeShareId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency (Mandant)
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Reference to Project
        public string? ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public SAAS_Projectplanningtool.Models.Budgetplanning.Project? Project { get; set; }

        // Reference to Employee (the Employee who has access to this Project)
        public string? EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

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