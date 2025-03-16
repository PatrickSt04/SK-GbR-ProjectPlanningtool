using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.Ressourceplanning
{
    public class Ressource
    {
        [Key]
        public required string RessourceId { get; set; } = Guid.NewGuid().ToString();

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


        // Latest Modifier of Database Entry
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModidier { get; set; }
    }
}
