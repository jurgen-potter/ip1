using CitizenPanel.BL.Domain.Panel;
using Microsoft.EntityFrameworkCore;
using CitizenPanel.DAL.Data;

namespace CitizenPanel.DAL;

public class PanelRepository : IPanelRepository
{
    private readonly PanelDbContext _dbContext;

    public PanelRepository(PanelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Panel ReadPanelById(int panelId)
    {
        return _dbContext.Panels.Find(panelId);
    }

    public IEnumerable<Panel> ReadAllPanels()
    {
        return _dbContext.Panels.ToList();
    }
    
    public void CreatePanel(Panel panel)
    {
        _dbContext.Panels.Add(panel);
        _dbContext.SaveChanges();
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