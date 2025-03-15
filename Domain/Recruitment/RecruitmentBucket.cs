using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL.Domain.Recruitment;

public class RecruitmentBucket
{
    public string Gender { get; set; }
    public string AgeGroup { get; set; }
    public int Count { get; set; }
    public int Target { get; set;}
}       