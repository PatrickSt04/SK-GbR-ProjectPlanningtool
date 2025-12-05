using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class ProjectTaskCatalogTask
    {
        [Key]
        [NotNull] public string ProjectTaskCatalogTaskId { get; set; } = Guid.NewGuid().ToString();
        public string? TaskName { get; set; }

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public string? ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        // Start Date of Project
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly? StartDate { get; set; }

        // End Date of Project
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly? EndDate { get; set; }

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


        public string? StateId { get; set; }
        [ForeignKey(nameof(StateId))]
        public SAAS_Projectplanningtool.Models.IndependentTables.State? State { get; set; }


        public string? ProjectTaskFixCostsId { get; set; }
        [ForeignKey(nameof(ProjectTaskFixCostsId))]
        public ProjectTaskFixCosts? ProjectTaskFixCosts { get; set; }

        /// <summary>
        /// Berechnet die Gesamtkosten basierend auf den Stundensatzgruppen und der Gesamtstundenzahl
        /// </summary>
        [NotMapped]
        public double? TotalCosts
        {
            get
            {
                double totalCosts = 0;
                //Wenn task nur TaskCatalogeintrag ist, dann werden Kosten über ProjectTaskFixCosts abgebildet
                
                    if (ProjectTaskFixCosts == null)
                        return null;

                    if (ProjectTaskFixCosts.FixCosts == null)
                        return null;

                    totalCosts = (double)ProjectTaskFixCosts?.FixCosts?.Where(p => p.Cost != null).Sum(p => p?.Cost);
                return totalCosts;
            }
        }
    }
}
