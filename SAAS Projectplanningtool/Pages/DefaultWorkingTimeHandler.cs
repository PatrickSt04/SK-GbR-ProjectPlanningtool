using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using System.ComponentModel.DataAnnotations;

namespace SAAS_Projectplanningtool.Pages
{
    public class DefaultWorkingTimeHandler
    {

        public WorkDayConfig Monday { get; set; } = new() { StartTime = new(8, 0), EndTime = new(17, 0) };
        public WorkDayConfig Tuesday { get; set; } = new() { StartTime = new(8, 0), EndTime = new(17, 0) };
        public WorkDayConfig Wednesday { get; set; } = new() { StartTime = new(8, 0), EndTime = new(17, 0) };
        public WorkDayConfig Thursday { get; set; } = new() { StartTime = new(8, 0), EndTime = new(17, 0) };
        public WorkDayConfig Friday { get; set; } = new() { StartTime = new(8, 0), EndTime = new(17, 0) };
        public WorkDayConfig Saturday { get; set; } = new() { StartTime = new(8, 0), EndTime = new(12, 0) };
        public WorkDayConfig Sunday { get; set; } = new() { StartTime = new(10, 0), EndTime = new(14, 0) };


        /// <summary>
        /// Gibt alle ausgewählten Arbeitstage als Liste zurück
        /// </summary>
        public List<int> GetSelectedWorkingDays()
        {
            var days = new List<int>();
            if (Monday.IsActive) days.Add(1);
            if (Tuesday.IsActive) days.Add(2);
            if (Wednesday.IsActive) days.Add(3);
            if (Thursday.IsActive) days.Add(4);
            if (Friday.IsActive) days.Add(5);
            if (Saturday.IsActive) days.Add(6);
            if (Sunday.IsActive) days.Add(7);
            return days;
        }

        /// <summary>
        /// Gibt die Arbeitszeiten als Dictionary zurück
        /// </summary>
        public Dictionary<int, WorkingHours> GetWorkingHours()
        {
            return new Dictionary<int, WorkingHours>
            {
                { 1, new WorkingHours { StartTime = Monday.StartTime, EndTime = Monday.EndTime } },
                { 2, new WorkingHours { StartTime = Tuesday.StartTime, EndTime = Tuesday.EndTime } },
                { 3, new WorkingHours { StartTime = Wednesday.StartTime, EndTime = Wednesday.EndTime } },
                { 4, new WorkingHours { StartTime = Thursday.StartTime, EndTime = Thursday.EndTime } },
                { 5, new WorkingHours { StartTime = Friday.StartTime, EndTime = Friday.EndTime } },
                { 6, new WorkingHours { StartTime = Saturday.StartTime, EndTime = Saturday.EndTime } },
                { 7, new WorkingHours { StartTime = Sunday.StartTime, EndTime = Sunday.EndTime } }
            };
        }

        /// <summary>
        /// Lädt Arbeitstage aus einer Liste
        /// </summary>
        public void LoadFromWorkingDays(List<int>? workingDays)
        {
            if (workingDays == null) return;

            Monday.IsActive = workingDays.Contains(1);
            Tuesday.IsActive = workingDays.Contains(2);
            Wednesday.IsActive = workingDays.Contains(3);
            Thursday.IsActive = workingDays.Contains(4);
            Friday.IsActive = workingDays.Contains(5);
            Saturday.IsActive = workingDays.Contains(6);
            Sunday.IsActive = workingDays.Contains(7);
        }

        /// <summary>
        /// Lädt Arbeitszeiten aus einem Dictionary
        /// </summary>
        public void LoadFromWorkingHours(Dictionary<int, WorkingHours>? workingHours)
        {
            if (workingHours == null) return;

            if (workingHours.ContainsKey(1)) Monday.LoadFrom(workingHours[1]);
            if (workingHours.ContainsKey(2)) Tuesday.LoadFrom(workingHours[2]);
            if (workingHours.ContainsKey(3)) Wednesday.LoadFrom(workingHours[3]);
            if (workingHours.ContainsKey(4)) Thursday.LoadFrom(workingHours[4]);
            if (workingHours.ContainsKey(5)) Friday.LoadFrom(workingHours[5]);
            if (workingHours.ContainsKey(6)) Saturday.LoadFrom(workingHours[6]);
            if (workingHours.ContainsKey(7)) Sunday.LoadFrom(workingHours[7]);
        }

        /// <summary>
        /// Validiert alle Arbeitszeiten
        /// </summary>
        public bool IsValid()
        {
            return Monday.IsValid() &&
                   Tuesday.IsValid() &&
                   Wednesday.IsValid() &&
                   Thursday.IsValid() &&
                   Friday.IsValid() &&
                   Saturday.IsValid() &&
                   Sunday.IsValid();
        }

        /// <summary>
        /// Setzt Standard-Arbeitszeiten (Mo-Fr 8:00-17:00)
        /// </summary>
        public void SetStandardWorkWeek()
        {
            Monday.IsActive = true;
            Tuesday.IsActive = true;
            Wednesday.IsActive = true;
            Thursday.IsActive = true;
            Friday.IsActive = true;
            Saturday.IsActive = false;
            Sunday.IsActive = false;

            foreach (var day in new[] { Monday, Tuesday, Wednesday, Thursday, Friday })
            {
                day.StartTime = new TimeOnly(8, 0);
                day.EndTime = new TimeOnly(17, 0);
            }
        }
    }

    /// <summary>
    /// Konfiguration für einen einzelnen Arbeitstag
    /// </summary>
    public class WorkDayConfig
    {
        public bool IsActive { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly StartTime { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; }

        public void LoadFrom(WorkingHours hours)
        {
            StartTime = hours.StartTime;
            EndTime = hours.EndTime;
        }

        public bool IsValid()
        {
            return StartTime < EndTime;
        }
    }
}