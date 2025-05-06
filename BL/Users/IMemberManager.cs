using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.BL.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Users;

public interface IMemberManager
{
    public Task<(IdentityResult result, ApplicationUser user)> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<int> newMemberSelectedCriteria, int newMemberPanelId);
    
    IEnumerable<ApplicationUser> GetAllMembers();
    ApplicationUser GetMemberById(string memberId);
    ApplicationUser GetOrganizationWithAnswers(string organizationId);
    void ChangeMember(ApplicationUser member);
    void RemoveMember(ApplicationUser member);

    public IEnumerable<Invitation> GetRegisteredInvitationsByPanelId(int panelId);


    /*// Specialized queries for the RegistrationManager
    IEnumerable<ApplicationUser> GetMembersByPanelId(int panelId);
    IEnumerable<ApplicationUser> GetMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);

    // Additional methods to support the RegistrationManager functionality
    int GetMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);*/

    Task ChangeOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers);
}