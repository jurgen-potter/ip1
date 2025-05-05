using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.DAL.Users;

public interface IMemberRepository
{
    IEnumerable<ApplicationUser> ReadAllMembers();
    ApplicationUser ReadUserById(string memberId);
    ApplicationUser ReadOrganizationWithAnswers(string organizationId);
    void UpdateMember(ApplicationUser member);
    void DeleteMember(ApplicationUser member);
    IEnumerable<ApplicationUser>ReadMembersOfPanelWithCriteria(int panelId);
    Task UpdateOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers);
}