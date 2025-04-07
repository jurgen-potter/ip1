using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL;

public class DrawRepository : IDrawRepository
{
    private readonly PanelDbContext _dbContext;

    public DrawRepository(PanelDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Invitation CreateInvitation(Invitation invitation)
    {
        return invitation;
    }

    public Invitation ReadInvitationWithCode(string code)
    {
        return _dbContext.Invitations.SingleOrDefault(i => i.Code == code);
    }

    public IEnumerable<Invitation> ReadAllInvitations()
    {
        return _dbContext.Invitations;
    }

    public Invitation UpdateInvitation(Invitation invitation)
    {
        _dbContext.Invitations.Update(invitation);
        _dbContext.SaveChanges();
        return invitation;
    }
    
    public ExtraCriteria ReadExtraCriteria(int criteriaId)
    {
        return _dbContext.ExtraCriteria
            .Where(e => e.Id == criteriaId)
            .Include(e => e.SubCriteria)
            .SingleOrDefault();
    }
    
    public IEnumerable<ExtraCriteria> ReadAllExtraCriteria()
    {
        return _dbContext.ExtraCriteria
            .Include(e => e.SubCriteria)
            .ToList();
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
    public bool DeleteInvitation(int invitationId)
    {
        var invitation = _dbContext.Invitations.Find(invitationId);
        if (invitation == null)
            return false;

        _dbContext.Invitations.Remove(invitation);
        return _dbContext.SaveChanges() > 0;
    }
}