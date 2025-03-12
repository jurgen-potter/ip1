namespace CitizenPanel.BL.Domain.Recruitment;

public class RecruitmentResult
{//Opslag van het berekende aantal panelleden per demografische groep.
  
    public int MaleCount { get; set; }
    public int FemaleCount { get; set; }

    public int Age18_25Count { get; set; }
    public int Age26_40Count { get; set; }
    public int Age41_60Count { get; set; }
    public int Age60PlusCount { get; set; }

    public List<CriteriaResult> ExtraCriteriaResults { get; set; }
}

public class CriteriaResult
{
    public string Name { get; set; }
    public List<SubCriteriaResult> SubResults { get; set; } = new List<SubCriteriaResult>();
}

public class SubCriteriaResult
{
    public string Name { get; set; }
    public int Count { get; set; }
}