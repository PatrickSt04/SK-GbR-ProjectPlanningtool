using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.Models.CRM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.CRM
{
    public enum CustomerType
    {
        Interessent = 0,
        Kunde = 1
    }

    public class Customer
    {
        [Key][NotNull] public string CustomerId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Address
        public string? AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address? Address { get; set; }

        // Core fields
        public required string CustomerName { get; set; }
        public CustomerType CustomerType { get; set; } = CustomerType.Interessent;

        // Contact details (Stammdaten)
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Website { get; set; }
        public string? Notes { get; set; }

        // Navigation collections
        public ICollection<CustomerContactPerson> ContactPersons { get; set; } = new List<CustomerContactPerson>();
        public ICollection<CustomerMessage> CustomerMessages { get; set; } = new List<CustomerMessage>();
        public ICollection<ContactHistoryEntry> ContactHistoryEntries { get; set; } = new List<ContactHistoryEntry>();
        public ICollection<CustomerMaterialSurcharge> MaterialSurcharges { get; set; } = new List<CustomerMaterialSurcharge>();
        public ICollection<Project>? Projects { get; set; }

        // Modification tracking
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModifier { get; set; }
        public DateTime? LatestModificationTimestamp { get; set; }
        public string? LatestModificationText { get; set; }

        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Employee? CreatedByEmployee { get; set; }
        public DateTime? CreatedTimestamp { get; set; }

        public bool DeleteFlag { get; set; } = false;
    }
}
