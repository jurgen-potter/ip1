using CitizenPanel.BL.Domain.Draw;
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
        return _dbContext.Panels
            .Include(p => p.DrawResult)
            .ThenInclude(dr => dr.SelectedMembers)
            .Include(p => p.DrawResult)
            .ThenInclude(dr => dr.ReserveMembers)
            .SingleOrDefault(p => p.PanelId == panelId);
    }

    public Panel ReadPanelByIdWithRecommendations(int panelId)
    {
        return _dbContext.Panels
            .Include(r => r.Recommendations)
            .SingleOrDefault(p => p.PanelId == panelId);
    }
    
    
    public void CreatePanel(Panel panel)
    {
        _dbContext.Panels.Add(panel);
    }

    public void UpdatePanel(Panel panel)
    {
        _dbContext.Update(panel);
        _dbContext.SaveChanges();
    }

    public void DeletePanel(Panel panel)
    {
        _dbContext.Panels.Remove(panel);
    }
    
    public IEnumerable<RecruitmentBucket> ReadTargetBucketsByPanel(Panel panel)
    {
        var panelWithBuckets = _dbContext.Panels
            .Include(p => p.RecruitmentBuckets)
            .FirstOrDefault(p => p.PanelId == panel.PanelId);
        return panelWithBuckets?.RecruitmentBuckets.ToList();
    }

    public void CreateRecommendationOfPanel(Recommendation recommendation, Panel panel)
    {
        panel.Recommendations.Add(recommendation);
        UpdatePanel(panel);
    }

    public Recommendation ReadRecommendationById(int recommendationId)
    {
        return _dbContext.Recommendations
            .Find(recommendationId);
    }

    public void UpdateRecommendation(Recommendation recommendation)
    {
        _dbContext.Update(recommendation);
    }
}