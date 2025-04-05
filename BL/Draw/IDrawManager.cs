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
    
    public ExtraCriteria GetExtraCriteria(int criteriaId);
    public IEnumerable<ExtraCriteria> GetAllExtraCriteria();
    public IEnumerable<ExtraCriteria> GetExtraCriteriaByPanel(int panelId);
    public SubCriteria GetSubCriteria(int subCriteriaId);

    public RecruitmentResult CalculateRecruitment(int totalAvailablePotentialPanelmembers, double malePercentage, double femalePercentage, double age18_25Percentage, double age26_40Percentage, double age41_60Percentage, double age60PlusPercentage, List<ExtraCriteria> extraCriteria);
    
    public bool RemoveInvitation(int invitationId);
}