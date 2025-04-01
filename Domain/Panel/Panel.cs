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
}