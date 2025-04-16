using SAAS_Projectplanningtool.Models.Budgetplanning;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Ressourceplanning
{
    public class ProjectTaskRessource
    {
        [Key]
        [NotNull] public string ProjectTaskRessourceId { get; set; } = Guid.NewGuid().ToString();
        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Ressource reference
        public string? RessourceId { get; set; }
        [ForeignKey(nameof(RessourceId))]
        public Ressource? Ressource { get; set; }

        // ProjectTask reference
        public string? ProjectTaskId { get; set; }
        [ForeignKey(nameof(ProjectTaskId))]
        public ProjectTask? ProjectTask { get; set; }

        public required float AmountPerUnit { get; set; }

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
