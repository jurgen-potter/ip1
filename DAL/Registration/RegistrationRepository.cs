using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Registration;

public class RegistrationRepository : IRegistrationRepository
{
    
    private readonly PanelDbContext _dbContext;
    
    public RegistrationRepository(PanelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ICollection<RecruitmentBucket> ReadTargetBucketsByPanel(Panel panel)
    {
        var panelWithBuckets = _dbContext.Panels
            .Include(p => p.RecruitmentBuckets)
            .FirstOrDefault(p => p.PanelId == panel.PanelId);
        if (panelWithBuckets == null)
        {
            return null;
        } //mss weg, momenteel hier gezet voor error te vermeiden
        return panelWithBuckets.RecruitmentBuckets.ToList();
    }

    public void updateDrawStatus(Panel panel)
    {
        panel.DrawStatus = DrawStatus.Complete;
        _dbContext.Update(panel);
    }
}