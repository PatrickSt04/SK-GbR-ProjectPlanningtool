using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.Budgetplanning
{
    public class ProjectTask
    {
        [Key]
        [NotNull] public string ProjectTaskId { get; set; } = Guid.NewGuid().ToString();
        public string? ProjectTaskName { get; set; }

        // Company dependency
        public string? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        // Reference to ProjectSection
        public string? ProjectSectionId { get; set; }
        [ForeignKey(nameof(ProjectSectionId))]
        public ProjectSection? ProjectSection { get; set; }

        // Start Date of Project
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly? StartDate { get; set; }

        // End Date of Project
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateOnly? EndDate { get; set; }

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

        // Amount of Workers per hourly rate group
        public ICollection<ProjectTaskHourlyRateGroup> ProjectTaskHourlyRateGroups { get; set; } = new List<ProjectTaskHourlyRateGroup>();


        // ---------- Berechnete Eigenschaften ----------

        /// <summary>
        /// Berechnet die Dauer der Task in Tagen basierend auf den Arbeitstagen des Projekts
        /// </summary>
        [NotMapped]
        public double? DurationInDays
        {
            get
            {
                if (!StartDate.HasValue || !EndDate.HasValue || StartDate > EndDate)
                    return null;

                var workDays = GetProjectWorkDays();
                if (workDays == null || !workDays.Any())
                    return null;

                return CalculateWorkingDays(StartDate.Value, EndDate.Value, workDays);
            }
        }

        /// <summary>
        /// Berechnet die Dauer der Task in Stunden basierend auf den Arbeitszeiten des Projekts
        /// </summary>
        [NotMapped]
        public double? DurationInHours
        {
            get
            {
                if (!StartDate.HasValue || !EndDate.HasValue || StartDate > EndDate)
                    return null;

                var workDays = GetProjectWorkDays();
                var workingHours = GetProjectWorkingHours();

                if (workDays == null || workingHours == null || !workDays.Any())
                    return null;

                return CalculateWorkingHours(StartDate.Value, EndDate.Value, workDays, workingHours);
            }
        }

        /// <summary>
        /// Berechnet die Gesamtkosten basierend auf den Stundensatzgruppen und der Gesamtstundenzahl
        /// </summary>
        [NotMapped]
        public double? TotalCosts
        {
            get
            {
                double totalCosts = 0;
                var totalHours = DurationInHours;
                if (!totalHours.HasValue || totalHours <= 0)
                    return null;

                if (ProjectTaskHourlyRateGroups == null || !ProjectTaskHourlyRateGroups.Any())
                    return null;


                var totalWorkers = ProjectTaskHourlyRateGroups.Sum(g => g.Amount);

                if (totalWorkers == 0)
                    return null;

                foreach (var group in ProjectTaskHourlyRateGroups)
                {
                    if (group.HourlyRateGroup?.HourlyRate != null && group.Amount > 0)
                    {
                        // Berechne die Stunden pro Arbeiter in dieser Gruppe
                        var hoursPerWorker = totalHours.Value;
                        var groupHours = hoursPerWorker * group.Amount;
                        var groupCosts = (double)groupHours * (double)group.HourlyRateGroup.HourlyRate;
                        totalCosts += groupCosts;
                    }
                }
                return totalCosts;
            }
        }

        // ---------- Private Hilfsmethoden ----------

        /// <summary>
        /// Holt die Arbeitstage des zugehörigen Projekts
        /// </summary>
        private List<int>? GetProjectWorkDays()
        {
            return ProjectSection?.Project?.DefaultWorkDays;
        }

        /// <summary>
        /// Holt die Arbeitszeiten des zugehörigen Projekts
        /// </summary>
        private Dictionary<int, WorkingHours>? GetProjectWorkingHours()
        {
            return ProjectSection?.Project?.DefaultWorkingHours;
        }

        /// <summary>
        /// Berechnet die Anzahl der Arbeitstage zwischen zwei Daten
        /// </summary>
        private double CalculateWorkingDays(DateOnly startDate, DateOnly endDate, List<int> workDays)
        {
            double workingDays = 0;
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                // Konvertiere .NET DayOfWeek (Sonntag = 0) zu ISO (Montag = 1)
                var dayOfWeek = currentDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)currentDate.DayOfWeek;

                if (workDays.Contains(dayOfWeek))
                {
                    workingDays++;
                }

                currentDate = currentDate.AddDays(1);
            }

            return workingDays;
        }

        /// <summary>
        /// Berechnet die Gesamtarbeitszeit in Stunden zwischen zwei Daten
        /// </summary>
        private double CalculateWorkingHours(DateOnly startDate, DateOnly endDate,
                                           List<int> workDays, Dictionary<int, WorkingHours> workingHours)
        {
            double totalHours = 0;
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                // Konvertiere .NET DayOfWeek (Sonntag = 0) zu ISO (Montag = 1)
                var dayOfWeek = currentDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)currentDate.DayOfWeek;

                if (workDays.Contains(dayOfWeek) && workingHours.ContainsKey(dayOfWeek))
                {
                    var hours = workingHours[dayOfWeek];
                    if (hours.StartTime < hours.EndTime)
                    {
                        var dailyHours = (hours.EndTime.ToTimeSpan() - hours.StartTime.ToTimeSpan()).TotalHours;
                        totalHours += dailyHours;
                    }
                }

                currentDate = currentDate.AddDays(1);
            }

            return totalHours;
        }

        // ---------- Zusätzliche Hilfsmethoden für erweiterte Berechnungen ----------

        /// <summary>
        /// Berechnet die durchschnittlichen Kosten pro Stunde über alle Stundensatzgruppen
        /// </summary>
        [NotMapped]
        public decimal? AverageHourlyCost
        {
            get
            {
                if (ProjectTaskHourlyRateGroups == null || !ProjectTaskHourlyRateGroups.Any())
                    return null;

                var totalWorkers = ProjectTaskHourlyRateGroups.Sum(g => g.Amount);
                if (totalWorkers == 0)
                    return null;

                decimal weightedSum = 0;
                foreach (var group in ProjectTaskHourlyRateGroups)
                {
                    if (group.HourlyRateGroup?.HourlyRate != null && group.Amount > 0)
                    {
                        weightedSum += (decimal)group.HourlyRateGroup.HourlyRate * (decimal)group.Amount;
                    }
                }

                return weightedSum / totalWorkers;
            }
        }

        /// <summary>
        /// Berechnet die Kosten pro Tag
        /// </summary>
        [NotMapped]
        public double? CostPerDay
        {
            get
            {
                var totalCosts = TotalCosts;
                var durationDays = DurationInDays;

                if (!totalCosts.HasValue || !durationDays.HasValue || durationDays <= 0)
                    return null;

                return totalCosts.Value / durationDays.Value;
            }
        }

        /// <summary>
        /// Gibt die Gesamtzahl der Arbeiter zurück
        /// </summary>
        [NotMapped]
        public int TotalWorkers
        {
            get
            {
                return ProjectTaskHourlyRateGroups?.Sum(g => g.Amount) ?? 0;
            }
        }

        /// <summary>
        /// Prüft ob alle erforderlichen Daten für Berechnungen vorhanden sind
        /// </summary>
        [NotMapped]
        public bool IsCalculationDataComplete
        {
            get
            {
                return StartDate.HasValue
                       && EndDate.HasValue
                       && StartDate <= EndDate
                       && ProjectSection?.Project != null
                       && ProjectTaskHourlyRateGroups != null
                       && ProjectTaskHourlyRateGroups.Any()
                       && ProjectTaskHourlyRateGroups.All(g => g.HourlyRateGroup?.HourlyRate != null);
            }
        }
    }
}