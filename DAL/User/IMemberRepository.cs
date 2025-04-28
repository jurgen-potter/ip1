using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

public interface IMemberRepository
{
    IEnumerable<ApplicationUser> ReadAllMembers();
    ApplicationUser ReadUserById(string memberId);
    void UpdateMember(ApplicationUser member);
    void DeleteMember(ApplicationUser member);
    
    // Specialized queries for the RegistrationManager
    IEnumerable<ApplicationUser> ReadMembersByPanelId(int panelId);
    IEnumerable<ApplicationUser> ReadMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);
    
    // Additional methods to support the RegistrationManager functionality
    int ReadMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);
}