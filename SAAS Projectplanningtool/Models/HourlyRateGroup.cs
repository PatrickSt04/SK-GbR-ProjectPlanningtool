using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models
{
    public class HourlyRateGroup
    {
        [Key]
        [NotNull] public string HourlyRateGroupId { get; set; } = Guid.NewGuid().ToString();
        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        // The hourly rate associated with this group
        public required float HourlyRate {  get; set; }
        // The name of the hourly rate group
        public required string HourlyRateGroupName { get; set; }

        // Latest Modifier of Database Entry
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModidier { get; set; }

    }
}
