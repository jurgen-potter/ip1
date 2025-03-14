namespace CitizenPanel.BL.Domain.Recruitment;

public class ExtraCriteria
{
    public string Name { get; set; } // Bijvoorbeeld "Transport"
    public List<SubCriteria> SubCriteria { get; set; } = new List<SubCriteria>();
}

public class SubCriteria
{
    public string Name { get; set; } // Bijvoorbeeld "OV", "Fiets"
    public double Percentage { get; set; }
}