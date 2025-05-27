using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.DAL.Draws;

namespace CitizenPanel.BL.Draws;

public class DrawManager(IDrawRepository repository, TenantContext tenantContext) : IDrawManager
{
    public Invitation AddInvitation(string code, string qrCodeString, int panelId, Gender gender, int age)
    {
        Invitation invitation = new Invitation()
        {
            Code = code,
            QRCodeString = qrCodeString,
            PanelId = panelId,
            Gender = gender,
            Age = age,
            Town = tenantContext.Tenant.Name,
            TenantId = tenantContext.Tenant.Id
        };
        repository.CreateInvitation(invitation);
        return invitation;
    }
    public Invitation GetInvitationById(int id)
    {
        return repository.ReadInvitationById(id);
    }

    public Invitation GetInvitationByCode(string code)
    {
        return repository.ReadInvitationByCode(code);
    }

    public IEnumerable<Invitation> GetAllInvitationsByPanelId(int panelId)
    {
        return repository.ReadInvitationsByPanelId(panelId);
    }
    
    public IEnumerable<Invitation> GetRegisteredInvitationsByPanelId(int panelId)
    {
        return repository.ReadRegisteredInvitationsByPanelId(panelId);
    }

    public bool EditInvitation(Invitation invitation)
    {
        return repository.UpdateInvitation(invitation);
    }
    
    public bool RemoveInvitation(int invitationId)
    {
        return repository.DeleteInvitation(invitationId);
    }

    public bool RemoveInvitationByEmail(string email)
    {
        return repository.DeleteInvitationByEmail(email);
    }

    public Criteria AddCriteria(string name, List<SubCriteria> subCriteria)
    {
        Criteria criteria = new Criteria()
        {
            Name = name,
            SubCriteria = subCriteria
        };
        repository.CreateCriteria(criteria);
        return criteria;
    }

    public SubCriteria AddSubCriteria(string name, double percentage)
    {
        SubCriteria subCriteria = new SubCriteria()
        {
            Name = name,
            Percentage = percentage
        };
        repository.CreateSubCriteria(subCriteria);
        return subCriteria;
    }
    
    public SubCriteria GetSubCriteria(int subCriteriaId)
    {
        return repository.ReadSubCriteriaById(subCriteriaId);
    }
}