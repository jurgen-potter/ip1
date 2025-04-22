using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL.Domain.Draw;

public class DrawResult
{
    public int Id { get; set; }
    public ICollection<Member> SelectedMembers { get; set; }
    public ICollection<Member> ReserveMembers { get; set; }
    public ICollection<Member> NotSelectedMembers { get; set; }

    public DrawResult()
    {
        SelectedMembers = new List<Member>();
        ReserveMembers = new List<Member>();
        NotSelectedMembers = new List<Member>();
    }
}