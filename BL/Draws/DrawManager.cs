using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.DAL.Draws;
using QRCoder;

namespace CitizenPanel.BL.Draws;

public class DrawManager(IDrawRepository drawRepository) : IDrawManager
{
    public Invitation AddInvitation(string code, string qrCodeString, int panelId, Gender gender, int age)
    {
        Invitation invitation = new Invitation()
        {
            Code = code,
            QRCodeString = qrCodeString,
            PanelId = panelId,
            Gender = gender,
            Age = age
        };
        drawRepository.CreateInvitation(invitation);
        return invitation;
    }
    
    public Invitation GetInvitationByCode(string code)
    {
        return drawRepository.ReadInvitationByCode(code);
    }

    public IEnumerable<Invitation> GetAllInvitationsByPanelId(int panelId)
    {
        return drawRepository.ReadInvitationsByPanelId(panelId);
    }

    public bool EditInvitation(Invitation invitation)
    {
        return drawRepository.UpdateInvitation(invitation);
    }
    
    public bool RemoveInvitation(int invitationId)
    {
        return drawRepository.DeleteInvitation(invitationId);
    }

    public bool RemoveInvitationByEmail(string email)
    {
        return drawRepository.DeleteInvitationByEmail(email);
    }

    public Criteria AddCriteria(string name, List<SubCriteria> subCriteria)
    {
        Criteria criteria = new Criteria()
        {
            Name = name,
            SubCriteria = subCriteria
        };
        drawRepository.CreateCriteria(criteria);
        return criteria;
    }

    public SubCriteria AddSubCriteria(string name, double percentage)
    {
        SubCriteria subCriteria = new SubCriteria()
        {
            Name = name,
            Percentage = percentage
        };
        drawRepository.CreateSubCriteria(subCriteria);
        return subCriteria;
    }
    
    public SubCriteria GetSubCriteria(int subCriteriaId)
    {
        return drawRepository.ReadSubCriteriaById(subCriteriaId);
    }
}