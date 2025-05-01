using CitizenPanel.BL.Domain.Tenancy;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Draw;

public class Criteria : ITenanted
{
    public int Id { get; set; }
    public string Name { get; set; } // Bijvoorbeeld "Transport"
    public IList<SubCriteria> SubCriteria { get; set; } = new List<SubCriteria>();
    public Panel.Panel Panel { get; set; }
    [Required]
    public string TenantId { get; set; }
}

public class SubCriteria : ITenanted
{
    public int Id { get; set; }
    public string Name { get; set; } // Bijvoorbeeld "OV", "Fiets"
    public double Percentage { get; set; }
    [Required]
    public string TenantId { get; set; }
    public int Count { get; set; }
}