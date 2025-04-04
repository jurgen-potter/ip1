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

    

    public void UpdateDrawStatus(Panel panel)
    {
        panel.DrawStatus = DrawStatus.Complete;
        _dbContext.Update(panel);
    }
}