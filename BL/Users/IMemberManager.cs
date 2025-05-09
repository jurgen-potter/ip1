using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.BL.Users;

public interface IMemberManager
{
    ApplicationUser GetMemberById(string memberId);
    
    ApplicationUser GetOrganizationByIdWithAnswers(string organizationId);
    Task ChangeOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers);
}