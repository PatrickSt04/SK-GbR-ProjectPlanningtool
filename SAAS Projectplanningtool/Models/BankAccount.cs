using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models
{
    public class BankAccount
    {
        [Key]
        [NotNull]
        public string BankAccountId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Kontobezeichnung")]
        public string AccountName { get; set; } = string.Empty;

        [Required]
        [StringLength(34)]
        [Display(Name = "IBAN")]
        public string IBAN { get; set; } = string.Empty;

        [StringLength(11)]
        [Display(Name = "BIC")]
        public string? BIC { get; set; }

        [StringLength(200)]
        [Display(Name = "Kreditinstitut")]
        public string? BankName { get; set; }

        [StringLength(200)]
        [Display(Name = "Kontoinhaber")]
        public string? AccountHolder { get; set; }

        // Latest Modification Attributes
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
