using CitizenPanel.BL.Domain.PanelManagement;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

public class PanelRepository : IPanelRepository
{
    private readonly PanelDbContext _dbContext;

    public PanelRepository(PanelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Panel GetPanelById(int panelId)
    {
        return _dbContext.Panels.Find(panelId);
    }

    public IEnumerable<Panel> GetAllPanels()
    {
        return _dbContext.Panels.ToList();
    }

    public void AddPanel(Panel panel)
    {
        _dbContext.Panels.Add(panel);
    }

    public void UpdatePanel(Panel panel)
    {
        _dbContext.Update(panel);
    }

    public void DeletePanel(Panel panel)
    {
        _dbContext.Panels.Remove(panel);
    }
}