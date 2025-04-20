using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL;

public interface IMemberManager
{
    public Task<(IdentityResult result, IdentityUser user)> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<int> newMemberSelectedCriteria, int newMemberPanelId);
    
    IEnumerable<Member> GetAllMembers();
    Member GetMemberById(string memberId);
    void ChangeMember(Member member);
    void RemoveMember(Member member);
    
    // Specialized queries for the RegistrationManager
    IEnumerable<Member> GetMembersByPanelId(int panelId);
    IEnumerable<Member> GetMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);
    
    // Additional methods to support the RegistrationManager functionality
    int GetMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);

}