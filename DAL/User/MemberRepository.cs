using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.QuestionnaireModule;
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
    
    public ApplicationUser ReadOrganizationWithAnswers(string organizationId)
    {
        return _dbContext.ApplicationUsers
            .Include(aU => aU.OrganizationProfile)
            .ThenInclude(op => op.Answers)
            .SingleOrDefault(au => au.Id == organizationId);
    }
    
    public void UpdateMember(ApplicationUser member)
    {
        _dbContext.Update(member);
    }

    public void DeleteMember(ApplicationUser member)
    {
        _dbContext.ApplicationUsers.Remove(member);
    }

    public async Task UpdateOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers)
    {
        var user = await _dbContext.Users
            .Include(u => u.OrganizationProfile)
            .ThenInclude(op => op.Answers)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        // Clear current answers
        var answersToRemove = user.OrganizationProfile.Answers
            .Where( a => a.Question != null && a.Question.Questionnaire?.Id == questionnaireId)
            .ToList(); // Materialize the query to avoid modifying collection during iteration

        foreach (var answer in answersToRemove)
        {
            user.OrganizationProfile.Answers.Remove(answer);
        }
        await _dbContext.SaveChangesAsync();

        // Add new answers
        foreach (var answer in answers)
        {
            user.OrganizationProfile.Answers.Add(answer);
        }
        
        _dbContext.Update(user);

        await _dbContext.SaveChangesAsync();
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
