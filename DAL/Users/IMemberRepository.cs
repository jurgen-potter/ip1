using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.DAL.Users;

public interface IMemberRepository
{
    ApplicationUser ReadUserById(string memberId);

    ApplicationUser ReadOrganizationByIdWithAnswers(string organizationId);
    Task UpdateOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers);
}