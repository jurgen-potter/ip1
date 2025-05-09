using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;

namespace CitizenPanel.BL.Utilities;

public interface IUtilityManager
{
    IEnumerable<Invitation> GenerateInvitations(int amount, List<Criteria> criteria, Panel panel);
    public RecruitmentResult CalculateRecruitment(int totalAvailablePotentialPanelmembers, IEnumerable<Criteria> criteriaList);
    public IEnumerable<Criteria> GetInitialCriteria();
}