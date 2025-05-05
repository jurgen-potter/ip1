using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Users;

public class MemberRepository(PanelDbContext dbContext) : IMemberRepository
{
    public IEnumerable<ApplicationUser> ReadAllMembers()
    {
        return dbContext.ApplicationUsers
            .Where(u => u.UserType == UserType.Member)
            .ToList();
    }

    public ApplicationUser ReadUserById(string userId)
    {
        return dbContext.ApplicationUsers.Find(userId);
    }
    
    public ApplicationUser ReadOrganizationWithAnswers(string organizationId)
    {
        return dbContext.ApplicationUsers
            .Include(aU => aU.OrganizationProfile)
            .ThenInclude(op => op.Answers)
            .SingleOrDefault(au => au.Id == organizationId);
    }
    
    public void UpdateMember(ApplicationUser member)
    {
        dbContext.Update(member);
    }

    public void DeleteMember(ApplicationUser member)
    {
        dbContext.ApplicationUsers.Remove(member);
    }

    public async Task UpdateOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers)
    {
        var user = await dbContext.Users
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
        await dbContext.SaveChangesAsync();

        // Add new answers
        foreach (var answer in answers)
        {
            user.OrganizationProfile.Answers.Add(answer);
        }
        
        dbContext.Update(user);

        await dbContext.SaveChangesAsync();
    }

    public IEnumerable<Invitation> ReadInvitationsByPanelId(int panelId)
    {
        return dbContext.Invitations
            .Where(inv => inv.PanelId == panelId)
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