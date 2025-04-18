using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL.Domain.Panel;

public class Panel
{
    public int PanelId { get; set; }
    public string Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int MemberCount { get; set; }
    public ICollection<Member> Members { get; set; }
    public ICollection<ExtraCriteria> ExtraCriteria { get; set; } = new List<ExtraCriteria>();
    public ICollection<RecruitmentBucket> RecruitmentBuckets { get; set; }
    public DrawStatus DrawStatus { get; set; }
    public DrawResult DrawResult { get; set; }
    public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
}