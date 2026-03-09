using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.CRM
{
    public class CustomerContactPerson
    {
        [Key] public string ContactPersonId { get; set; } = Guid.NewGuid().ToString();

        public string CustomerId { get; set; } = default!;
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        public required string Name { get; set; }
        public string? Role { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }
        public bool IsPrimary { get; set; } = false;
    }
}
