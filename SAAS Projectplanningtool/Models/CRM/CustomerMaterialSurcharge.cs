using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.CRM
{
    public class CustomerMaterialSurcharge
    {
        [Key] public string SurchargeId { get; set; } = Guid.NewGuid().ToString();

        public string CustomerId { get; set; } = default!;
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        [Required]
        public required string MaterialCategory { get; set; }

        public string? Description { get; set; }

        [Range(0, 1000, ErrorMessage = "Zuschlag muss zwischen 0 und 1000% liegen.")]
        public decimal SurchargePercent { get; set; } = 0;
    }
}
