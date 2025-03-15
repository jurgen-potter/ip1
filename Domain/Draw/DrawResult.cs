using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL.Domain.Draw;

public class DrawResult
{
    public List<Member> SelectedMembers { get; set; }
    public List<Member> ReserveMembers { get; set; }

    public DrawResult()
    {
        SelectedMembers = new List<Member>();
        ReserveMembers = new List<Member>();
    }
}