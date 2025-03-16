using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models
{
    public class Employee
    {
        [Key] 
        public string EmployeeId { get; set; } = Guid.NewGuid().ToString();
        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        // Reference to the ASP.NET Identity User table
        public required string IdentityUserId { get; set; }
        [ForeignKey(nameof(IdentityUserId))]
        public IdentityUser? IdentityUser { get; set; }
        // Reference to the ASP.NET Identity Role table
        public required string IdentityRoleId { get; set; }
        [ForeignKey(nameof(IdentityRoleId))]
        public IdentityRole? IdentityRole { get; set; }
        // Reference to the HourlyRateGroup
        public required string HourlyRateGroupId { get; set; }
        [ForeignKey(nameof(HourlyRateGroupId))] 
        public HourlyRateGroup? HourlyRateGroup { get; set; }

        public required string EmployeeDisplayName { get; set; }
    }
}
