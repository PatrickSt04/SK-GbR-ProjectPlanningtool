using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.Ressourceplanning
{
    public class Unit
    {
        [Key]
        public required string UnitId { get; set; } = Guid.NewGuid().ToString();
        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Latest Modifier of Database Entry
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModidier { get; set; }

        public required string UnitName { get; set; }
    }
}
