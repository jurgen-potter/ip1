using CitizenPanel.BL.Domain.PanelManagement;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

using BL.Domain.Recruitment;
using Microsoft.EntityFrameworkCore;

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
    
    public ExtraCriteria ReadExtraCriteria(int criteriaId)
    {
        return _dbContext.ExtraCriteria
            .Where(e => e.Id == criteriaId)
            .Include(e => e.SubCriteria)
            .SingleOrDefault();
    }
    
    public List<ExtraCriteria> ReadAllExtraCriteria()
    {
        return _dbContext.ExtraCriteria
            .Include(e => e.SubCriteria)
            .ToList();
    }

    public void AddPanel(Panel panel)
    {
        _dbContext.Panels.Add(panel);
    }

    public void UpdatePanel(Panel panel)
    {
        _dbContext.Update(panel);
    }

    public SubCriteria ReadSubCriteria(int subCriteriaId)
    {
        return _dbContext.SubCriteria.Find(subCriteriaId);
    }


    public void DeletePanel(Panel panel)
    {
        _dbContext.Panels.Remove(panel);
    }
}