using SAAS_Projectplanningtool.Models.IndependentTables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models
{
    public class Company
    {
        [Key]
        [NotNull] public string CompanyId { get; set; } = Guid.NewGuid().ToString();

        // The name of the company
        public required string CompanyName { get; set; }

        // The industry sector in which the company operates
        public string? SectorId { get; set; }
        [ForeignKey(nameof(SectorId))]
        public IndustrySector? Sector { get; set; }

        // The license chosen by the company
        public string? LicenseId { get; set; }
        [ForeignKey(nameof(LicenseId))]
        public LicenseModel? License { get; set; }

        // The company's headquarters address
        // Can be initialized or modified later
        public string? AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address? Address { get; set; }

        // The company's default working days
        // Represented as numbers from 1 to 7 (Monday - Sunday)
        // Can be initialized or modified later
        public List<int>? DefaultWorkDays { get; set; }
    }
}
