using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.IndependentTables
{
    public class LicenseModel
    {
        // Represents all available license models, independent of any specific company
        [Key]
        [NotNull] public string LicenseId { get; set; } = Guid.NewGuid().ToString();

        // The name of the license model
        public required string LicenseName { get; set; }
    }
}
