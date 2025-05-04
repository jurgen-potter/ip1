using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL;

public interface IMemberManager
{
    public Task<(IdentityResult result, ApplicationUser user)> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<int> newMemberSelectedCriteria, int newMemberPanelId);
    
    IEnumerable<ApplicationUser> GetAllMembers();
    ApplicationUser GetMemberById(string memberId);
    void ChangeMember(ApplicationUser member);
    void RemoveMember(ApplicationUser member);
    
    /*// Specialized queries for the RegistrationManager
    IEnumerable<ApplicationUser> GetMembersByPanelId(int panelId);
    IEnumerable<ApplicationUser> GetMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);
    
    // Additional methods to support the RegistrationManager functionality
    int GetMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);*/

    Task ChangeOrganizationAnswersAsync(string userId, List<Answer> answers);
}