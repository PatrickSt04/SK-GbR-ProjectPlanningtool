using System.ComponentModel.DataAnnotations;

namespace SAAS_Projectplanningtool.Models.IndependentTables
{
    public class License
    {
        // Represents all available license models, independent of any specific company
        [Key]
        public required string LicenseId { get; set; } = Guid.NewGuid().ToString();

        // The name of the license model
        public required string LicenseName { get; set; }
    }
}
