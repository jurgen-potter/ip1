using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

public interface IMemberRepository
{
    IEnumerable<Member> GetAllMembers();
    Member GetMemberById(int memberId);
    void AddMember(Member member);
    void UpdateMember(Member member);
    void DeleteMember(Member member);
    
    // Specialized queries for the RegistrationManager
    IEnumerable<Member> GetMembersByPanelId(int panelId);
    IEnumerable<Member> GetMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);
    
    // Additional methods to support the RegistrationManager functionality
    int GetMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge);
    void MarkMembersAsSelected(IEnumerable<Member> members);
}