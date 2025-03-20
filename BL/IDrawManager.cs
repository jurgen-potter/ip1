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

    public RecruitmentResult CalculateRecruitment(RecruitmentCriteria criteria);
    
    IEnumerable<RecruitmentBucket> GetInvitationBuckets(Panel panel);
    IEnumerable<RecruitmentBucket> GetAllBuckets(Panel panel);
    
    DrawStatus GetDrawStatus(Panel panel);
    bool StartFinalDraw(Panel panel);
    DrawResult PerformFinalDraw(Panel panel);
    bool HasSufficientRegistrations(Panel panel);
    DrawResult GetDrawResults(Panel panel);

}