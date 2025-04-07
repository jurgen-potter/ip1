using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL.Domain.Draw;

public class DrawResult
{
    public int Id { get; set; }
    public List<Member> SelectedMembers { get; set; }
    public List<Member> ReserveMembers { get; set; }
    
    public List<Member> NotSelectedMembers { get; set; }

    public DrawResult()
    {
        SelectedMembers = new List<Member>();
        ReserveMembers = new List<Member>();
        NotSelectedMembers = new List<Member>();
    }
}