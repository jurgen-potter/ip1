using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Draw;

public class ExtraCriteria
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } // Bijvoorbeeld "Transport"
    public List<SubCriteria> SubCriteria { get; set; } = new List<SubCriteria>();
    public Panel.Panel Panel { get; set; }
}

public class SubCriteria
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } // Bijvoorbeeld "OV", "Fiets"
    public double Percentage { get; set; }
}