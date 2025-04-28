namespace CitizenPanel.BL.Domain.Draw;

public class RecruitmentBucket
{
    public List<string> CriteriaNames { get; set; } = new List<string>();
    public List<string> SubCriteriaNames { get; set; } = new List<string>();
    public int Count { get; set; }
    
    public string Description =>
        string.Join(" × ", SubCriteriaNames
            .Select((sub, idx) => $"{sub} ({CriteriaNames[idx]})"));
}