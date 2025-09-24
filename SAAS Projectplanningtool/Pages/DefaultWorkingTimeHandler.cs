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

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        // Working Days properties for binding
        [BindProperty]
        public bool Monday { get; set; }
        [BindProperty]
        public bool Tuesday { get; set; }
        [BindProperty]
        public bool Wednesday { get; set; }
        [BindProperty]
        public bool Thursday { get; set; }
        [BindProperty]
        public bool Friday { get; set; }
        [BindProperty]
        public bool Saturday { get; set; }
        [BindProperty]
        public bool Sunday { get; set; }

        // Working Hours properties for binding
        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly MondayStart { get; set; } = new TimeOnly(8, 0);
        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly MondayEnd { get; set; } = new TimeOnly(17, 0);

        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly TuesdayStart { get; set; } = new TimeOnly(8, 0);
        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly TuesdayEnd { get; set; } = new TimeOnly(17, 0);

        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly WednesdayStart { get; set; } = new TimeOnly(8, 0);
        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly WednesdayEnd { get; set; } = new TimeOnly(17, 0);

        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly ThursdayStart { get; set; } = new TimeOnly(8, 0);
        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly ThursdayEnd { get; set; } = new TimeOnly(17, 0);

        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly FridayStart { get; set; } = new TimeOnly(8, 0);
        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly FridayEnd { get; set; } = new TimeOnly(17, 0);

        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly SaturdayStart { get; set; } = new TimeOnly(8, 0);
        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly SaturdayEnd { get; set; } = new TimeOnly(12, 0);

        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly SundayStart { get; set; } = new TimeOnly(10, 0);
        [BindProperty]
        [DataType(DataType.Time)]
        public TimeOnly SundayEnd { get; set; } = new TimeOnly(14, 0);



        public DefaultWorkingTimeHandler(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void LoadWorkingDaysToProperties(List<int>? workingDays)
        {
            if (workingDays == null) return;

            Monday = workingDays.Contains(1);
            Tuesday = workingDays.Contains(2);
            Wednesday = workingDays.Contains(3);
            Thursday = workingDays.Contains(4);
            Friday = workingDays.Contains(5);
            Saturday = workingDays.Contains(6);
            Sunday = workingDays.Contains(7);
        }

        public void LoadWorkingHoursToProperties(Dictionary<int, WorkingHours> workingHours)
        {
            if (workingHours.ContainsKey(1)) { MondayStart = workingHours[1].StartTime; MondayEnd = workingHours[1].EndTime; }
            if (workingHours.ContainsKey(2)) { TuesdayStart = workingHours[2].StartTime; TuesdayEnd = workingHours[2].EndTime; }
            if (workingHours.ContainsKey(3)) { WednesdayStart = workingHours[3].StartTime; WednesdayEnd = workingHours[3].EndTime; }
            if (workingHours.ContainsKey(4)) { ThursdayStart = workingHours[4].StartTime; ThursdayEnd = workingHours[4].EndTime; }
            if (workingHours.ContainsKey(5)) { FridayStart = workingHours[5].StartTime; FridayEnd = workingHours[5].EndTime; }
            if (workingHours.ContainsKey(6)) { SaturdayStart = workingHours[6].StartTime; SaturdayEnd = workingHours[6].EndTime; }
            if (workingHours.ContainsKey(7)) { SundayStart = workingHours[7].StartTime; SundayEnd = workingHours[7].EndTime; }
        }

        public List<int> GetSelectedWorkingDays()
        {
            var workingDays = new List<int>();
            if (Monday) workingDays.Add(1);
            if (Tuesday) workingDays.Add(2);
            if (Wednesday) workingDays.Add(3);
            if (Thursday) workingDays.Add(4);
            if (Friday) workingDays.Add(5);
            if (Saturday) workingDays.Add(6);
            if (Sunday) workingDays.Add(7);
            return workingDays;
        }

        public Dictionary<int, WorkingHours> GetWorkingHoursFromProperties()
        {
            return new Dictionary<int, WorkingHours>
            {
                { 1, new WorkingHours { StartTime = MondayStart, EndTime = MondayEnd } },
                { 2, new WorkingHours { StartTime = TuesdayStart, EndTime = TuesdayEnd } },
                { 3, new WorkingHours { StartTime = WednesdayStart, EndTime = WednesdayEnd } },
                { 4, new WorkingHours { StartTime = ThursdayStart, EndTime = ThursdayEnd } },
                { 5, new WorkingHours { StartTime = FridayStart, EndTime = FridayEnd } },
                { 6, new WorkingHours { StartTime = SaturdayStart, EndTime = SaturdayEnd } },
                { 7, new WorkingHours { StartTime = SundayStart, EndTime = SundayEnd } }
            };
        }

        public bool ValidateWorkingHours()
        {
            return MondayStart < MondayEnd &&
                   TuesdayStart < TuesdayEnd &&
                   WednesdayStart < WednesdayEnd &&
                   ThursdayStart < ThursdayEnd &&
                   FridayStart < FridayEnd &&
                   SaturdayStart < SaturdayEnd &&
                   SundayStart < SundayEnd;
        }

        public void LoadDefaultWorkTimesOfCompany(string? companyId)
        {
            if (string.IsNullOrEmpty(companyId)) return;

            var company = _context.Company
                .FirstOrDefault(c => c.CompanyId == companyId);

            if (company != null)
            {
                // List<int> ist direkt aus DB lesbar (falls EF Core das unterstützt, siehe unten)
                LoadWorkingDaysToProperties(company.DefaultWorkDays);

                // Dictionary wird automatisch aus JSON-String umgewandelt
                LoadWorkingHoursToProperties(company.DefaultWorkingHours);
            }
        }

    }
}
