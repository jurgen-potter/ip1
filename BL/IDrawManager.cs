using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

public interface IDrawManager
{
    public List<Invitation> AddInvitations(List<DummyMember> members);
}