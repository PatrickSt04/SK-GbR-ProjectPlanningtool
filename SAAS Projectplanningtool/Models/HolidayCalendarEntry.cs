using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models
{
    public class HolidayCalendarEntry
    {
        [Key]
        [NotNull]
        public string HolidayCalendarEntryId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Name")]
        public string HolidayName { get; set; } = string.Empty;

        [Display(Name = "Datum")]
        public DateOnly HolidayDate { get; set; }

        [Display(Name = "Art")]
        public HolidayEntryType HolidayType { get; set; } = HolidayEntryType.PublicHoliday;

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

    public enum HolidayEntryType
    {
        [Display(Name = "Feiertag")]
        PublicHoliday = 1,

        [Display(Name = "Betriebsurlaub")]
        CompanyVacation = 2
    }
}
