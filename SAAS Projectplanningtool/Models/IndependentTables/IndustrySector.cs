using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.IndependentTables
{
    public class IndustrySector
    {
        // Represents all available industry sectors, independent of any specific company
        [Key]
        [NotNull] public string SectorId { get; set; } = Guid.NewGuid().ToString();

        // The name of the industry sector
        public required string SectorName { get; set; }
    }
}
