using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class ProjectTaskFixCosts
    {
        [Key]
        public string ProjectTaskFixCostsId { get; set; } = Guid.NewGuid().ToString();
        public string TaskId { get; set; } = default!;
        [ForeignKey(nameof(TaskId))]
        public ProjectTaskCatalogTask ProjectTaskCatalogTask { get; set; } = default!;

        public List<FixCost>? FixCosts { get; set; } = new();
        //[Owned]
        public class FixCost
        {
            public string? Description { get; set; }
            public double? Cost { get; set; }
        }

    }
}
