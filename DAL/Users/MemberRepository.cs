using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Users;

public class MemberRepository(PanelDbContext dbContext) : IMemberRepository
{
    public ApplicationUser ReadUserById(string userId)
    {
        return dbContext.ApplicationUsers.Find(userId);
    }
    
    public ApplicationUser ReadOrganizationByIdWithAnswers(string organizationId)
    {
        return dbContext.ApplicationUsers
            .Include(aU => aU.OrganizationProfile)
            .ThenInclude(op => op.Answers)
            .SingleOrDefault(au => au.Id == organizationId);
    }
    
    public async Task UpdateOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers)
    {
        var user = ReadOrganizationByIdWithAnswers(userId);
        
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
}