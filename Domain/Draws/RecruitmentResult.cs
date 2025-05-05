namespace CitizenPanel.BL.Domain.Draws;

public class RecruitmentResult
{
    public int ReservePotPanelmembers {get; set;}
    public int TotalNeededPanelmembers {get; set;}
    public List<RecruitmentBucket> Buckets { get; set; } = new List<RecruitmentBucket>();
}

public class RecruitmentBucket
{
    public List<string> CriteriaNames { get; set; } = new List<string>();
    public List<string> SubCriteriaNames { get; set; } = new List<string>();
    public int Count { get; set; }
    
    public int ActualCount { get; set; }

}