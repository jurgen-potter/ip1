using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Users;

public class UserProfileRepository(PanelDbContext dbContext) : IUserProfileRepository
{
    public ApplicationUser ReadUserById(string userId)
    {
        return dbContext.Users.Find(userId);
    }
    
    public ApplicationUser ReadUserByIdWithProfile(string userId)
    {
        return dbContext.Users
            .Include(u => u.MemberProfile)
            .Include(u => u.OrganizationProfile)
            .IgnoreQueryFilters()
            .SingleOrDefault(u => u.Id == userId);
    }
    
    public ApplicationUser ReadUserByIdWithProfileAndPanels(string userId)
    {
        return dbContext.Users
            .Include(u => u.MemberProfile)
            .ThenInclude(m => m.Panels)
            .Include(u => u.OrganizationProfile)
            .IgnoreQueryFilters()
            .SingleOrDefault(u => u.Id == userId);
    }
    
    public ApplicationUser ReadOrganizationByIdWithProfileAndAnswers(string organizationId)
    {
        return dbContext.Users
            .Include(aU => aU.OrganizationProfile)
            .ThenInclude(op => op.Answers)
            .SingleOrDefault(au => au.Id == organizationId);
    }
    public IEnumerable<ApplicationUser> ReadAllOrganizations()
    {
        return dbContext.OrganizationProfiles
            .Select(u => u.ApplicationUser)
            .Where(u => u.UserType == UserType.Organization)
            .ToList();
    }
    public IEnumerable<MemberProfile> ReadAllOrganizationMembersNotInPanel(int panelId)
    {
        return dbContext.MemberProfiles
            .Include(u => u.ApplicationUser)
            .Where(u => u.ApplicationUser.UserType == UserType.Member)
            .Where(u => u.Panels.All(p => p.Id != panelId))
            .Where(u => u.ApplicationUser.IsStaff == true)
            .ToList();
    }

    public async Task UpdateOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers)
    {
        var user = ReadOrganizationByIdWithProfileAndAnswers(userId);
        
        var answersToRemove = user.OrganizationProfile.Answers
            .Where( a => a.Question != null && a.Question.Questionnaire?.Id == questionnaireId)
            .ToList();

        foreach (var answer in answersToRemove)
        {
            user.OrganizationProfile.Answers.Remove(answer);
        }
        await dbContext.SaveChangesAsync();

        foreach (var answer in answers)
        {
            user.OrganizationProfile.Answers.Add(answer);
        }
        
        dbContext.Update(user);

        await dbContext.SaveChangesAsync();
    }
    
    public IEnumerable<ApplicationUser> ReadAllAdmins()
    {
        return dbContext.Users
            .Where(u => u.UserType == UserType.Admin)
            .ToList();
    }
}