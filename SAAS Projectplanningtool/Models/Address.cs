using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models
{
    public class Address
    {
        [Key]
        public required string AddressId { get; set; } = Guid.NewGuid().ToString();
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

        // Latest Modifier of Database Entry
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModidier { get; set; }
    }
}
