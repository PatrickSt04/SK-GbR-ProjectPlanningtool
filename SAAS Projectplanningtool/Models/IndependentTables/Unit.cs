using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;


namespace SAAS_Projectplanningtool.Models.IndependentTables;

public class Unit
{
    [Key]
    [NotNull] public string UnitId { get; set; } = Guid.NewGuid().ToString();
    
    public string? Name { get; set; }
    
    public string? ShortName { get; set; }  
}