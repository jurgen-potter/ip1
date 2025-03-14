using CitizenPanel.BL.Domain.Draw;

namespace CitizenPanel.DAL;

//NugetPackage nog toevoegen aan DAL
public interface IDrawRepository
{
    public Invitation AddInvitation(Invitation invitation);
}