using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.DAL.Users;

public interface IUserProfileRepository
{
    ApplicationUser ReadUserById(string userId);
    ApplicationUser ReadUserByIdWithProfile(string userId);
    ApplicationUser ReadUserByIdWithProfileAndPanels(string userId);

    ApplicationUser ReadOrganizationByIdWithProfileAndAnswers(string organizationId);
    Task UpdateOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers);
    
    IEnumerable<ApplicationUser> ReadAllAdmins();
}