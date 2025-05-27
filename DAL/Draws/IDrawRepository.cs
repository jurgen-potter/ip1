using CitizenPanel.BL.Domain.Draws;

namespace CitizenPanel.DAL.Draws;

public interface IDrawRepository
{
    void CreateInvitation(Invitation invitation);
    Invitation ReadInvitationById(int id);
    Invitation ReadInvitationByCode(string code);
    IEnumerable<Invitation> ReadInvitationsByPanelId(int panelId);
    IEnumerable<Invitation> ReadRegisteredInvitationsByPanelId(int panelId);
    bool UpdateInvitation(Invitation invitation);
    bool DeleteInvitation(int invitationId);
    bool DeleteInvitationByEmail(string email);
    
    void CreateCriteria(Criteria criteria);
    
    void CreateSubCriteria(SubCriteria subCriteria);
    SubCriteria ReadSubCriteriaById(int subCriteriaId);
}