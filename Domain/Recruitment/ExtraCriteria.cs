namespace CitizenPanel.BL.Domain.Recruitment;

public class ExtraCriteria
{
    public int Id { get; set; }
    public string Name { get; set; } // Bijvoorbeeld "Transport"
    public List<SubCriteria> SubCriteria { get; set; } = new List<SubCriteria>();
}

public class SubCriteria
{
    public int Id { get; set; }
    public string Name { get; set; } // Bijvoorbeeld "OV", "Fiets"
    public double Percentage { get; set; }
}