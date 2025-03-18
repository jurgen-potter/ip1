using CitizenPanel.BL.Domain.Draw;

namespace CitizenPanel.DAL;

//NugetPackage nog toevoegen aan DAL
public interface IDrawRepository
{
    public Invitation AddInvitation(Invitation invitation);
    public Invitation ReadInvitationWithCode(string code);
    public IEnumerable<Invitation> ReadAllInvitations();
    public Invitation UpdateInvitation(Invitation invitation);
}