using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Draws;

public class DrawRepository(PanelDbContext dbContext) : IDrawRepository
{
    public void CreateInvitation(Invitation invitation)
    {
        dbContext.Invitations.Add(invitation);
        dbContext.SaveChanges();
    }
    public Invitation ReadInvitationById(int id)
    {
        return dbContext.Invitations.Find(id);
    }

    public Invitation ReadInvitationByCode(string code)
    {
        return dbContext.Invitations
            .SingleOrDefault(i => i.Code == code);
    }

    public IEnumerable<Invitation> ReadInvitationsByPanelId(int panelId)
    {
        return dbContext.Invitations
            .Where(i => i.PanelId == panelId)
            .ToList();
    }

    public IEnumerable<Invitation> ReadRegisteredInvitationsByPanelId(int panelId)
    {
        return dbContext.Invitations
            .Where(inv => inv.PanelId == panelId)
            .Where(inv => inv.IsRegistered == true)
            .ToList();
    }
    
    public bool UpdateInvitation(Invitation invitation)
    {
        dbContext.Invitations.Update(invitation);
        return dbContext.SaveChanges() > 0;
    }
    
    public bool DeleteInvitation(int invitationId)
    {
        var invitation = dbContext.Invitations.Find(invitationId);
        if (invitation == null)
            return false;

        dbContext.Invitations.Remove(invitation);
        return dbContext.SaveChanges() > 0;
    }

    public bool DeleteInvitationByEmail(string email)
    {
        var invitation = dbContext.Invitations.SingleOrDefault(i => i.Email == email);
        if (invitation == null)
            return false;
        
        dbContext.Invitations.Remove(invitation);
        return dbContext.SaveChanges() > 0;
    }

    public void CreateCriteria(Criteria criteria)
    {
        dbContext.Criteria.Add(criteria);
        dbContext.SaveChanges();
    }
    
    public void CreateSubCriteria(SubCriteria subCriteria)
    {
        dbContext.SubCriteria.Add(subCriteria);
        dbContext.SaveChanges();
    }
    
    public SubCriteria ReadSubCriteriaById(int subCriteriaId)
    {
        return dbContext.SubCriteria.Find(subCriteriaId);
    }
}