using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.DAL.Data;

namespace CitizenPanel.DAL;

public class DrawRepository : IDrawRepository
{
    private readonly PanelDbContext _context;

    public DrawRepository(PanelDbContext context)
    {
        _context = context;
    }
    
    public Invitation AddInvitation(Invitation invitation)
    {
        return invitation;
    }

    public Invitation ReadInvitationWithCode(string code)
    {
        return _context.Invitations.SingleOrDefault(i => i.Code == code);
    }

    public IEnumerable<Invitation> ReadAllInvitations()
    {
        return _context.Invitations;
    }

    public Invitation UpdateInvitation(Invitation invitation)
    {
        _context.Invitations.Update(invitation);
        _context.SaveChanges();
        return invitation;
    }
}