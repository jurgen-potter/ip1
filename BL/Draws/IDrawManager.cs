using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.BL.Draws;

public interface IDrawManager
{
    Invitation AddInvitation(string code, string qrCodeString, int panelId, Gender gender, int age);
    Invitation GetInvitationByCode(string code);
    IEnumerable<Invitation> GetAllInvitationsByPanelId(int panelId);
    bool EditInvitation(Invitation invitation);
    bool RemoveInvitation(int invitationId);
    bool RemoveInvitationByEmail(string email);
    
    Criteria AddCriteria(string name, List<SubCriteria> subCriteria);
    
    SubCriteria AddSubCriteria(string name, double percentage);
    SubCriteria GetSubCriteria(int subCriteriaId);
}