using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

public class PanelRepository : IPanelRepository
{
    private readonly PanelDbContext _context;

    public PanelRepository(PanelDbContext context)
    {
        _context = context;
    }


    public Panelmember CreatePanelmember(Panelmember panelmember)
    {
        return panelmember;
    }
}