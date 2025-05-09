using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.BL.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Users;

public interface IMemberManager
{
    ApplicationUser GetMemberById(string memberId);
    
    ApplicationUser GetOrganizationByIdWithAnswers(string organizationId);
    Task ChangeOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers);
}