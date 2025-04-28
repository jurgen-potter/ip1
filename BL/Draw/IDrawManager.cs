using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

public interface IDrawManager
{
    public IEnumerable<Invitation> AddInvitations(List<DummyMember> members);
    public Invitation GetInvitationWithCode(string code);
    public IEnumerable<Invitation> GetAllInvitations();
    public Invitation ChangeInvitation(Invitation invitation);
    
    public Criteria AddCriteria(string name, List<SubCriteria> subCriteria);
    public SubCriteria AddSubCriteria(string name, double percentage);
    public Criteria GetCriteria(int criteriaId);
    public IEnumerable<Criteria> GetAllCriteria();
    public IEnumerable<Criteria> GetCriteriaByPanel(int panelId);
    public SubCriteria GetSubCriteria(int subCriteriaId);

    public RecruitmentResult CalculateRecruitment(int totalAvailablePotentialPanelmembers, double malePercentage, double femalePercentage, double age18_25Percentage, double age26_40Percentage, double age41_60Percentage, double age60PlusPercentage, List<Criteria> extraCriteria);
    
    public bool RemoveInvitation(int invitationId);
}