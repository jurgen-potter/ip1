using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.BL.Domain.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CitizenPanel.BL.Users;

public interface IUserProfileManager
{
    ApplicationUser GetUserById(string memberId);
    ApplicationUser GetUserByIdWithProfile(string memberId);
    ApplicationUser GetUserByPrincipalWithProfileAndPanels(ClaimsPrincipal userPrincipal);
    
    ApplicationUser GetOrganizationByIdWithProfileAndAnswers(string organizationId);
    Task EditOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers);
}