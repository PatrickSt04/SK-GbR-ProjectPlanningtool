using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models
{
    public class Logfile
    {
        [Key] public string LogfileId { get; set; } = Guid.NewGuid().ToString();
        public string? ExceptionName { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? ExcecutingEmployeeId { get; set; }
        [ForeignKey(nameof(ExcecutingEmployeeId))]
        public Employee? ExcecutingEmployee { get; set; }
        public DateTime? TimeOfException { get; set; }
        public string? ExceptionPath { get; set; }

        public string? CustomMessage { get; set; }

        public string? SerializedObject { get; set; }
    }
}
