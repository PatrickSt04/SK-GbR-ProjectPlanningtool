using System.ComponentModel.DataAnnotations;
using SAAS_Projectplanningtool.Models.IndependentTables;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models;

/// <summary>
/// Junction table: which Units a Company has activated.
/// </summary>
public class CompanyUnit
{
    [Key]
    public string CompanyUnitId { get; set; } = Guid.NewGuid().ToString();
    public string CompanyId { get; set; } = default!;

    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }

    public string UnitId { get; set; } = default!;

    [ForeignKey(nameof(UnitId))]
    public Unit? Unit { get; set; }
}