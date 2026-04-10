using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SAAS_Projectplanningtool.Models.IndependentTables;

public class Unit
{
    [Key]
    [NotNull]
    public string UnitId { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(100)]
    [Display(Name = "Bezeichnung")]
    public string? Name { get; set; }

    [StringLength(20)]
    [Display(Name = "Kurzzeichen")]
    public string? ShortName { get; set; }
}