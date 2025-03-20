using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL.Data;

namespace CitizenPanel.DAL;

public class MemberRepository : IMemberRepository
{
    private readonly PanelDbContext _dbContext;

    public MemberRepository(PanelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Member> ReadAllMembers()
    {
        return _dbContext.Members.ToList();
    }

    public Member ReadMemberById(int memberId)
    {
        return _dbContext.Members.Find(memberId);
    }
    
    public void UpdateMember(Member member)
    {
        _dbContext.Update(member);
    }

    public void DeleteMember(Member member)
    {
        _dbContext.Members.Remove(member);
    }

    public IEnumerable<Member> ReadMembersByPanelId(int panelId)
    {
        return _dbContext.Members
            .Where(m => m.Panel.PanelId == panelId)
            .ToList();
    }

    public IEnumerable<Member> ReadMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge,
        int maxAge)
    {
        return _dbContext.Members
            .Where(m => m.Panel.PanelId == panelId &&
                        m.Gender == gender &&
                        m.Age >= minAge &&
                        m.Age <= maxAge)
            .ToList();
    }

    public int ReadMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge)
    {
        return _dbContext.Members
            .Count(m => m.Panel.PanelId == panelId &&
                        m.Gender == gender &&
                        m.Age >= minAge &&
                        m.Age <= maxAge);
    }

}
