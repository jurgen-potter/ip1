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

    public IEnumerable<Member> GetAllMembers()
    {
        return _dbContext.Members.ToList();
    }

    public Member GetMemberById(int memberId)
    {
        return _dbContext.Members.Find(memberId);
    }

    public void AddMember(Member member)
    {
        _dbContext.Members.Add(member);
    }

    public void UpdateMember(Member member)
    {
        _dbContext.Update(member);
    }

    public void DeleteMember(Member member)
    {
        _dbContext.Members.Remove(member);
    }

    public IEnumerable<Member> GetMembersByPanelId(int panelId)
    {
        return _dbContext.Members
            .Where(m => m.Panel.PanelId == panelId)
            .ToList();
    }

    public IEnumerable<Member> GetMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge,
        int maxAge)
    {
        return _dbContext.Members
            .Where(m => m.Panel.PanelId == panelId &&
                        m.Gender == gender &&
                        m.Age >= minAge &&
                        m.Age <= maxAge)
            .ToList();
    }

    public int GetMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge)
    {
        return _dbContext.Members
            .Count(m => m.Panel.PanelId == panelId &&
                        m.Gender == gender &&
                        m.Age >= minAge &&
                        m.Age <= maxAge);
    }

    public void MarkMembersAsSelected(IEnumerable<Member> members)
    {
        foreach (var member in members)
        {
            member.IsSelected = true;
            UpdateMember(member);
            _dbContext.SaveChanges();

        }
    }
}
