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

    public IEnumerable<Invitation> ReadAllInvitationsByPanelId(int panelId)
    {
        return _dbContext.Invitations.Where(i => i.PanelId == panelId);
    }

    public Invitation UpdateInvitation(Invitation invitation)
    {
        _dbContext.Invitations.Update(invitation);
        _dbContext.SaveChanges();
        return invitation;
    }

    public SubCriteria CreateSubCriteria(SubCriteria subCriteria)
    {
        _dbContext.SubCriteria.Add(subCriteria);
        _dbContext.SaveChanges();
        return subCriteria;
    }

    public Criteria CreateCriteria(Criteria criteria)
    {
        _dbContext.Criteria.Add(criteria);
        _dbContext.SaveChanges();
        return criteria;
    }

    public Criteria ReadCriteria(int criteriaId)
    {
        return _dbContext.Criteria
            .Where(e => e.Id == criteriaId)
            .Include(e => e.SubCriteria)
            .SingleOrDefault();
    }
    
    public IEnumerable<Criteria> ReadAllCriteria()
    {
        return _dbContext.Criteria
            .Include(e => e.SubCriteria)
            .ToList();
    }
    
    public SubCriteria ReadSubCriteria(int subCriteriaId)
    {
        return _dbContext.SubCriteria.Find(subCriteriaId);
    }

    public IEnumerable<Criteria> ReadCriteriaByPanel(int panelId)
    {
        return _dbContext.Criteria
            .Where(e => e.Panel.Id == panelId)
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

    public bool DeleteInvitationByEmail(string email)
    {
        var invitation = _dbContext.Invitations.SingleOrDefault(i => i.Email == email);
        if (invitation == null)
            return false;
        
        _dbContext.Invitations.Remove(invitation);
        return _dbContext.SaveChanges() > 0;
    }

    public void UpdateCriteria(int panelId, IEnumerable<Criteria> updatedCriteria)
    {
        // haal alle bestaande criteria voor deze panel
        var existing = _dbContext.Criteria
            .Include(c => c.SubCriteria)
            .Where(c => c.Panel.Id == panelId)
            .ToList();

        // verwijder criteria die verdwenen zijn
        foreach (var toRemove in existing.Where(e => updatedCriteria.All(u => u.Id != e.Id)))
        {
            _dbContext.Criteria.Remove(toRemove);
        }

        // verwerk elke criteria uit de aangeleverde lijst
        foreach (var upd in updatedCriteria)
        {
            if (upd.Id == 0)
            {
                // nieuw
                var crit = new Criteria {
                    Panel = _dbContext.Panels.Find(panelId),
                    Name = upd.Name
                };
                foreach (var sc in upd.SubCriteria)
                    crit.SubCriteria.Add(new SubCriteria {
                        Name = sc.Name,
                        Percentage = sc.Percentage
                    });
                _dbContext.Criteria.Add(crit);
            }
            else
            {
                // bestaand bijwerken
                var existCrit = existing.Single(e => e.Id == upd.Id);
                existCrit.Name = upd.Name;

                // subcriteria: eerst alle oude weghalen
                _dbContext.SubCriteria.RemoveRange(existCrit.SubCriteria);
                // en vervangen door de nieuwe
                existCrit.SubCriteria = upd.SubCriteria
                    .Select(sc => new SubCriteria {
                        Name = sc.Name,
                        Percentage = sc.Percentage
                    }).ToList();
            }
        }

        _dbContext.SaveChanges();
    }

}