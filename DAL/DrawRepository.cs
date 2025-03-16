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
}