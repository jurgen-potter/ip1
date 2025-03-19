using CitizenPanel.BL.Domain.PanelManagement;
using CitizenPanel.BL.Domain.Recruitment;
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

    public void CreatePanel(Panel panel)
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

    public IEnumerable<ExtraCriteria> ReadExtraCriteriaByPanel(int panelId)
    {
        return _dbContext.ExtraCriteria
            .Where(e => e.Panel.PanelId == panelId)
            .Include(e => e.SubCriteria)
            .ToList();
    }


    public void DeletePanel(Panel panel)
    {
        _dbContext.Panels.Remove(panel);
    }
}