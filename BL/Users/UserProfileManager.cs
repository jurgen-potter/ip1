using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.DAL.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CitizenPanel.BL.Users;

public class UserProfileManager(
    IUserProfileRepository userProfileRepository,
    UserManager<ApplicationUser> userManager) : IUserProfileManager
{
    public ApplicationUser GetUserById(string memberId)
    {
        return userProfileRepository.ReadUserById(memberId);
    }
    
    public ApplicationUser GetUserByIdWithProfile(string userId)
    {
        return userProfileRepository.ReadUserByIdWithProfile(userId);
    }
    
    public ApplicationUser GetUserByPrincipalWithProfileAndPanels(ClaimsPrincipal userPrincipal)
    {
        var userId = userManager.GetUserId(userPrincipal);
        return userProfileRepository.ReadUserByIdWithProfileAndPanels(userId);
    }
    
    public ApplicationUser GetOrganizationByIdWithProfileAndAnswers(string organizationId)
    {
        return userProfileRepository.ReadOrganizationByIdWithProfileAndAnswers(organizationId);
    }

    public Task EditOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers)
    {
        return userProfileRepository.UpdateOrganizationAnswersAsync(userId, questionnaireId, answers);
    }
    
    public IEnumerable<ApplicationUser> GetAllAdmins()
    {
        return userProfileRepository.ReadAllAdmins();
    }
}