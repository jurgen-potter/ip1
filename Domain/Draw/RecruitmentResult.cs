namespace CitizenPanel.BL.Domain.Draw;

public class RecruitmentResult
{//Opslag van het berekende aantal panelleden per demografische groep.
    public int ReservePotPanelmembers {get; set;}
    public double TotalNeededPanelmembers {get; set;}
    public List<CriteriaResult> CriteriaResults { get; set; }
    public List<RecruitmentBucket> Buckets { get; set; } = new List<RecruitmentBucket>();

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