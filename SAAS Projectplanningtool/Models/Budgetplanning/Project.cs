using Mono.TextTemplating;
using SAAS_Projectplanningtool.Models.IndependentTables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class Project
    {
        [Key]
        [NotNull] public string ProjectId { get; set; } = Guid.NewGuid().ToString();

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Reference to Customer of this Project
        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        // Reference to ProjectBudget of this Project
        public string? ProjectBudgetId { get; set; }
        [ForeignKey(nameof(ProjectBudgetId))]
        public ProjectBudget? ProjectBudget { get; set; }

        public required string ProjectName { get; set; }
        public required string ProjectDescription { get; set; }

        // Start Date of Project
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly? StartDate { get; set; }

        // End Date of Project
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly? EndDate { get; set; }
        // Instead of Deleting a completed Project it is archived 
        public bool? IsArchived { get; set; }

        // Responsible person 
        // Reference to Employee
        public string? ResponsiblePersonId { get; set; }
        [ForeignKey(nameof(ResponsiblePersonId))]
        public Employee? ResponsiblePerson { get; set; }

        public string? StateId { get; set; }
        [ForeignKey(nameof(StateId))]
        public SAAS_Projectplanningtool.Models.IndependentTables.State? State { get; set; }
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

        public ICollection<ProjectSection>? ProjectSections { get; set; }

        // The project's default working days
        // Represented as numbers from 1 to 7 (Monday - Sunday)
        public List<int>? DefaultWorkDays { get; set; } = new(new int[] { 1, 2, 3, 4, 5 });

        // Default working hours - stored as JSON string in database
        [Column(TypeName = "nvarchar(max)")]
        public string? DefaultWorkingHoursJson { get; set; }

        // Helper property to work with working hours in code (not mapped to database)
        [NotMapped]
        public Dictionary<int, WorkingHours> DefaultWorkingHours
        {
            get
            {
                if (string.IsNullOrEmpty(DefaultWorkingHoursJson))
                {
                    return GetDefaultWorkingHours();
                }
                try
                {
                    return JsonSerializer.Deserialize<Dictionary<int, WorkingHours>>(DefaultWorkingHoursJson) ?? GetDefaultWorkingHours();
                }
                catch
                {
                    return GetDefaultWorkingHours();
                }
            }
            set
            {
                DefaultWorkingHoursJson = JsonSerializer.Serialize(value);
            }
        }

        // Default working hours (Monday to Friday: 08:00-17:00)
        private Dictionary<int, WorkingHours> GetDefaultWorkingHours()
        {
            return new Dictionary<int, WorkingHours>
            {
                { 1, new WorkingHours { StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(17, 0) } }, // Monday
                { 2, new WorkingHours { StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(17, 0) } }, // Tuesday
                { 3, new WorkingHours { StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(17, 0) } }, // Wednesday
                { 4, new WorkingHours { StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(17, 0) } }, // Thursday
                { 5, new WorkingHours { StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(17, 0) } }, // Friday
                { 6, new WorkingHours { StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0) } }, // Saturday
                { 7, new WorkingHours { StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(14, 0) } }  // Sunday
            };
        }
    }
}
