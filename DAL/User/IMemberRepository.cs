using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

public interface IMemberRepository
{
    IEnumerable<Member> ReadAllMembers();
    Member ReadMemberById(string memberId);
    void UpdateMember(Member member);
    void DeleteMember(Member member);
    
    // Specialized queries for the RegistrationManager
    IEnumerable<Member> ReadMembersByPanelId(int panelId);
    IEnumerable<Member> ReadMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);
    
    // Additional methods to support the RegistrationManager functionality
    int ReadMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);
}