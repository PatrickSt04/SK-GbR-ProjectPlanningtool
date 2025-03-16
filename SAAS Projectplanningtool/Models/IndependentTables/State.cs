using System.ComponentModel.DataAnnotations;

namespace SAAS_Projectplanningtool.Models.IndependentTables
{
    public class State
    {
        [Key]
        public string StateId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Display(Name = "Status")]
        public string StateName { get; set; }

        [Required]
        [Display(Name = "Farbe #XXXXXX")]
        public string Color { get; set; }

    }
}
