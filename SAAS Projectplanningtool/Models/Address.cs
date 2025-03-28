using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models
{
    public class Address
    {
        [Key]
        [NotNull] public string AddressId { get; set; } = Guid.NewGuid().ToString();
        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))] 
        public Company? Company { get; set; }

        // The street name of the address
        public string? Street { get; set; }

        // The house or building number
        public string? HouseNumber { get; set; }

        // The city where the address is located
        public string? City { get; set; }

        // The region, state, or province of the address
        public string? Region { get; set; }

        // The postal or ZIP code
        public string? PostalCode { get; set; }

        // The country of the address
        public string? Country { get; set; }

        // Latest Modification Attributes
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModifier { get; set; }

        public DateTime? LatestModificationTimestamp { get; set; }
        public string? LatestModificationText { get; set; }
    }
}
