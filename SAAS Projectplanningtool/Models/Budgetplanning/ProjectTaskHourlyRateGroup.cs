using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class ProjectTaskHourlyRateGroup
    {
        [Key]
        public string ProjectTaskHourlyRateGroupId { get; set; } = Guid.NewGuid().ToString();

        public string ProjectTaskId { get; set; } = default!;
        [ForeignKey(nameof(ProjectTaskId))]
        public ProjectTask ProjectTask { get; set; } = default!;

        public string HourlyRateGroupId { get; set; } = default!;
        [ForeignKey(nameof(HourlyRateGroupId))]
        public HourlyRateGroup HourlyRateGroup { get; set; } = default!;

        public int Amount { get; set; }
    }
}
