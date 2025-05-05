using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Tenancy;

namespace CitizenPanel.BL.Domain.Draws;

public class Criteria : ITenanted
{
    public int Id { get; set; }
    public string Name { get; set; } // Bijvoorbeeld "Transport"
    public IList<SubCriteria> SubCriteria { get; set; } = new List<SubCriteria>();
    public Panel Panel { get; set; }
    public string TenantId { get; set; }
}

public class SubCriteria : ITenanted
{
    public int Id { get; set; }
    public string Name { get; set; } // Bijvoorbeeld "OV", "Fiets"
    public double Percentage { get; set; }
    public string TenantId { get; set; }
    public int Count { get; set; }
}