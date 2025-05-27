using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;

namespace CitizenPanel.BL.Utilities;

public interface IUtilityManager
{
    IEnumerable<Invitation> GenerateInvitations(int amount, List<Criteria> criteria, Panel panel);
    int CalculateMembers(int totalAvailablMembers);
    RecruitmentResult CalculateRecruitment(int totalToDraw, IEnumerable<Criteria> criteriaList);
    IEnumerable<Criteria> GetInitialCriteria();
}