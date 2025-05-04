using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL;

public class MemberRepository : IMemberRepository
{
    private readonly PanelDbContext _dbContext;

    public MemberRepository(PanelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<ApplicationUser> ReadAllMembers()
    {
        return _dbContext.ApplicationUsers
            .Where(u => u.UserType == UserType.Member)
            .ToList();
    }

    public ApplicationUser ReadUserById(string userId)
    {
        return _dbContext.ApplicationUsers.Find(userId);
    }

    public void UpdateMember(ApplicationUser member)
    {
        _dbContext.Update(member);
    }

    public void DeleteMember(ApplicationUser member)
    {
        _dbContext.ApplicationUsers.Remove(member);
    }

    public IEnumerable<MemberProfile> ReadMembersOfPanelWithCriteria(int panelId)
    {
        return _dbContext.MemberProfiles
            .Include(mp => mp.SelectedCriteria)   
            .Include(mp => mp.Panels)
            .Where(mp => mp.Panels.Any(p => p.Id == panelId))
            .ToList();
    }



/*public IEnumerable<ApplicationUser> ReadMembersByPanelId(int panelId)
{
    return _dbContext.ApplicationUsers
        .Where(u => u.MemberProfile.Panel.Id == panelId)
        .ToList();
}

public IEnumerable<ApplicationUser> ReadMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge,
    int maxAge)
{
    return _dbContext.ApplicationUsers
        .Where(m => m.MemberProfile.Panel.Id == panelId &&
                    m.MemberProfile.Gender == gender &&
                    m.MemberProfile.Age >= minAge &&
                    m.MemberProfile.Age <= maxAge)
        .ToList();
}

public int ReadMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge)
{
    return _dbContext.ApplicationUsers
        .Count(m => m.MemberProfile.Panel.Id == panelId &&
                    m.MemberProfile.Gender == gender &&
                    m.MemberProfile.Age >= minAge &&
                    m.MemberProfile.Age <= maxAge);
}*/
}