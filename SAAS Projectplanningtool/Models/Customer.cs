using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models
{
    public class Customer
    {
        [Key] public string CustomerId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        // The customers address
        public required string AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address? Address { get; set; }
        // The customers name
        public required string CustomerName { get; set; }

        // Latest Modifier of Database Entry
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModidier { get; set; }
    }
}
