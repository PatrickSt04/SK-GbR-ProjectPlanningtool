using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models
{
    public class Employee
    {
        [Key]
        [NotNull] public string EmployeeId { get; set; } = Guid.NewGuid().ToString();
        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        // Reference to the ASP.NET Identity User table
        public string? IdentityUserId { get; set; }
        [ForeignKey(nameof(IdentityUserId))]
        public IdentityUser? IdentityUser { get; set; }
        // Reference to the ASP.NET Identity Role table
        public string? IdentityRoleId { get; set; }
        [ForeignKey(nameof(IdentityRoleId))]
        public IdentityRole? IdentityRole { get; set; }
        // Reference to the HourlyRateGroup
        public string? HourlyRateGroupId { get; set; }
        [ForeignKey(nameof(HourlyRateGroupId))] 
        public HourlyRateGroup? HourlyRateGroup { get; set; }

        public required string EmployeeDisplayName { get; set; }

        // Latest Modification Attributes
        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModifierEmployee { get; set; }

        public DateTime? LatestModificationTimestamp { get; set; }
        public string? LatestModificationText { get; set; }

        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Employee? CreatedByEmployee { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
    }
}
