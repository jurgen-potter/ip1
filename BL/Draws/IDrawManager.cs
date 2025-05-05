using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;

namespace CitizenPanel.BL.Draws;

public interface IDrawManager
{
    public IEnumerable<Invitation> AddInvitations(int amount, List<Criteria> criteria, Panel panel);
    public Invitation GetInvitationWithCode(string code);
    public IEnumerable<Invitation> GetAllInvitationsByPanelId(int panelId);
    public Invitation ChangeInvitation(Invitation invitation);

    public Criteria GetCriteria(int criteriaId);
    public IEnumerable<Criteria> GetAllCriteria();
    public IEnumerable<Criteria> GetCriteriaByPanel(int panelId);
    public SubCriteria GetSubCriteria(int subCriteriaId);
    public Criteria AddCriteria(string name, List<SubCriteria> subCriteria);
    public SubCriteria AddSubCriteria(string name, double percentage);
    public void EditCriteria(int PanelId, IEnumerable<Criteria> criteria);

    public RecruitmentResult CalculateRecruitment(
        int totalAvailablePotentialPanelmembers,
        IEnumerable<Criteria> criteriaList
    );

    public bool RemoveInvitation(int invitationId);
    public bool RemoveInvitationByEmail(string email);
    public IEnumerable<Criteria> GetInitialCriteria();
}