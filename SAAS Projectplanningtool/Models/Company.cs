using SAAS_Projectplanningtool.Models.IndependentTables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SAAS_Projectplanningtool.Models
{
    public class Company
    {
        [Key]
        [NotNull] public string CompanyId { get; set; } = Guid.NewGuid().ToString();

        // The name of the company
        public required string CompanyName { get; set; }

        // The industry sector in which the company operates
        public string? SectorId { get; set; }
        [ForeignKey(nameof(SectorId))]
        public IndustrySector? Sector { get; set; }

        // The license chosen by the company
        public string? LicenseId { get; set; }
        [ForeignKey(nameof(LicenseId))]
        public LicenseModel? License { get; set; }

        // The company's headquarters address
        public string? AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address? Address { get; set; }

        // The company's default working days
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
    }

    public class WorkingHours
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public bool IsValid => StartTime < EndTime;

        public TimeSpan Duration => EndTime.ToTimeSpan() - StartTime.ToTimeSpan();

        public string DisplayText => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
    }
}