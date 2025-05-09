using CitizenPanel.BL.Domain.Draws;

namespace CitizenPanel.DAL.Draws;

public interface IDrawRepository
{
    public void CreateInvitation(Invitation invitation);
    public Invitation ReadInvitationByCode(string code);
    public IEnumerable<Invitation> ReadInvitationsByPanelId(int panelId);
    public bool UpdateInvitation(Invitation invitation);
    public bool DeleteInvitation(int invitationId);
    public bool DeleteInvitationByEmail(string email);
    
    public void CreateCriteria(Criteria criteria);
    
    public void CreateSubCriteria(SubCriteria subCriteria);
    public SubCriteria ReadSubCriteriaById(int subCriteriaId);
}