using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Draws;

public class DrawRepository(PanelDbContext dbContext) : IDrawRepository
{
    public Invitation CreateInvitation(Invitation invitation)
    {
        return invitation;
    }

    public Invitation ReadInvitationWithCode(string code)
    {
        return dbContext.Invitations.SingleOrDefault(i => i.Code == code);
    }

    public IEnumerable<Invitation> ReadAllInvitationsByPanelId(int panelId)
    {
        return dbContext.Invitations.Where(i => i.PanelId == panelId);
    }

    public Invitation UpdateInvitation(Invitation invitation)
    {
        dbContext.Invitations.Update(invitation);
        dbContext.SaveChanges();
        return invitation;
    }

    public SubCriteria CreateSubCriteria(SubCriteria subCriteria)
    {
        dbContext.SubCriteria.Add(subCriteria);
        dbContext.SaveChanges();
        return subCriteria;
    }

    public Criteria CreateCriteria(Criteria criteria)
    {
        dbContext.Criteria.Add(criteria);
        dbContext.SaveChanges();
        return criteria;
    }

    public Criteria ReadCriteria(int criteriaId)
    {
        return dbContext.Criteria
            .Where(e => e.Id == criteriaId)
            .Include(e => e.SubCriteria)
            .SingleOrDefault();
    }
    
    public IEnumerable<Criteria> ReadAllCriteria()
    {
        return dbContext.Criteria
            .Include(e => e.SubCriteria)
            .ToList();
    }
    
    public SubCriteria ReadSubCriteria(int subCriteriaId)
    {
        return dbContext.SubCriteria.Find(subCriteriaId);
    }

    public IEnumerable<Criteria> ReadCriteriaByPanel(int panelId)
    {
        return dbContext.Criteria
            .Where(e => e.Panel.Id == panelId)
            .Include(e => e.SubCriteria)
            .ToList();
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

    public void UpdateCriteria(int panelId, IEnumerable<Criteria> updatedCriteria)
    {
        // haal alle bestaande criteria voor deze panel
        var existing = dbContext.Criteria
            .Include(c => c.SubCriteria)
            .Where(c => c.Panel.Id == panelId)
            .ToList();

        // verwijder criteria die verdwenen zijn
        foreach (var toRemove in existing.Where(e => updatedCriteria.All(u => u.Id != e.Id)))
        {
            dbContext.Criteria.Remove(toRemove);
        }

        // verwerk elke criteria uit de aangeleverde lijst
        foreach (var upd in updatedCriteria)
        {
            if (upd.Id == 0)
            {
                // nieuw
                var crit = new Criteria {
                    Panel = dbContext.Panels.Find(panelId),
                    Name = upd.Name
                };
                foreach (var sc in upd.SubCriteria)
                    crit.SubCriteria.Add(new SubCriteria {
                        Name = sc.Name,
                        Percentage = sc.Percentage
                    });
                dbContext.Criteria.Add(crit);
            }
            else
            {
                // bestaand bijwerken
                var existCrit = existing.Single(e => e.Id == upd.Id);
                existCrit.Name = upd.Name;

                // subcriteria: eerst alle oude weghalen
                dbContext.SubCriteria.RemoveRange(existCrit.SubCriteria);
                // en vervangen door de nieuwe
                existCrit.SubCriteria = upd.SubCriteria
                    .Select(sc => new SubCriteria {
                        Name = sc.Name,
                        Percentage = sc.Percentage
                    }).ToList();
            }
        }

        dbContext.SaveChanges();
    }

}