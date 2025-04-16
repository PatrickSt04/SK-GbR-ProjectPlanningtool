using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Ressourceplanning
{
    public class Ressource
    {
        [Key]
        [NotNull] public string RessourceId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Reference to RessourceType
        public string? RessourceTypeId { get; set; }
        [ForeignKey(nameof(RessourceTypeId))]
        public RessourceType? RessourceType { get; set; }

        // Reference to Unit
        public string? UnitId { get; set; }
        [ForeignKey(nameof(UnitId))]
        public Unit? Unit { get; set; }

        public required float CostsPerUnit { get; set; }

        public required string RessourceName { get; set; }


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
