using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SAAS_Projectplanningtool.Models.Budgetplanning;

namespace SAAS_Projectplanningtool.Models.TimeTracking
{
    public class TimeEntry
    {
        [Key]
        [NotNull] public string TimeEntryId { get; set; } = Guid.NewGuid().ToString();

        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public string ProjectId { get; set; } = default!;
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        public string EmployeeId { get; set; } = default!;
        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly WorkDate { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public int BreakMinutes { get; set; } = 0;

        [NotMapped]
        public double NetWorkingHours
        {
            get
            {
                var totalMinutes = (EndTime - StartTime).TotalMinutes - BreakMinutes;
                return Math.Max(0, totalMinutes / 60.0);
            }
        }


        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? LatestModifierId { get; set; }
        [ForeignKey(nameof(LatestModifierId))]
        public Employee? LatestModifier { get; set; }

        public DateTime? LatestModificationTimestamp { get; set; }
        public string? LatestModificationText { get; set; }

        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Employee? CreatedByEmployee { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
    }
}