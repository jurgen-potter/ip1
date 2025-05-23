using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Panels;

public class PanelRepository(PanelDbContext dbContext) : IPanelRepository
{
    public void CreatePanel(Panel panel)
    {
        dbContext.Panels.Add(panel);
        dbContext.SaveChanges();
    }

    public IEnumerable<Panel> ReadAllPanels()
    {
        return dbContext.Panels.ToList();
    }

    public IEnumerable<Panel> ReadAllPanelsWithoutTentant()
    {
        return dbContext.Panels
            .IgnoreQueryFilters()
            .Where(p => p.IsActive)
            .ToList();
    }

    public Panel ReadPanelById(int panelId)
    {
        return dbContext.Panels.Find(panelId);
    }

    public IEnumerable<Panel> ReadThreeActivePanels()
    {
        return dbContext.Panels.Where(p => p.IsActive).Take(3).IgnoreQueryFilters().ToList();
    }

    public Panel ReadPanelByIdWithMembers(int panelId)
    {
        return dbContext.Panels
            .Include(p => p.Members)
            .ThenInclude(m => m.ApplicationUser)
            .SingleOrDefault(p => p.Id == panelId);
    }

    public Panel ReadPanelByIdWithInvitations(int panelId)
    {
        return dbContext.Panels
            .Include(p => p.Invitations)
            .Include(p => p.DrawResult)
            .SingleOrDefault(p => p.Id == panelId);
    }

    public Panel ReadPanelByIdWithRecommendations(int panelId)
    {
        return dbContext.Panels
            .Include(r => r.Meetings)
            .ThenInclude(m => m.Recommendations)
            .SingleOrDefault(p => p.Id == panelId);
    }

    public Panel ReadPanelByIdWithAcceptedRecommendationsAndPosts(int panelId)
    {
        return dbContext.Panels
            .Include(p => p.Posts)
            .Include(p => p.Meetings)
            .ThenInclude(m => m.Recommendations.Where(rec => rec.Accepted == true))
            .SingleOrDefault(p => p.Id == panelId);
    }


    public Panel ReadPanelByIdWithRecommendationsAndVotes(int panelId)
    {
        return dbContext.Panels
            .Include(r => r.Meetings)
            .ThenInclude(m => m.Recommendations)
            .ThenInclude(r => r.UserVotes)
            .SingleOrDefault(p => p.Id == panelId);
    }

    public void UpdatePanel(Panel panel)
    {
        dbContext.Update(panel);
        dbContext.SaveChanges();
    }

    public void DeletePanel(Panel panel)
    {
        dbContext.Panels.Remove(panel);
    }

    public Recommendation ReadRecommendationByIdWithVotes(int recommendationId)
    {
        return dbContext.Recommendations
            .Include(r => r.UserVotes)
            .SingleOrDefault(r => r.Id == recommendationId);
    }

    public Recommendation ReadRecommendationByIdWithVoters(int recommendationId)
    {
        return dbContext.Recommendations
            .Include(r => r.UserVotes)
            .ThenInclude(uv => uv.Voter)
            .ThenInclude(v => v.MemberProfile)
            .SingleOrDefault(r => r.Id == recommendationId);
    }

    public IEnumerable<Recommendation> ReadVotableRecommendationsByIdWithVotes(int panelId)
    {
        var meetings = dbContext.Meetings.Include(m => m.Recommendations).ThenInclude(r => r.UserVotes).Where(m => m.PanelId == panelId).ToList();
        
        List<Recommendation> recommendations = new List<Recommendation>();
        foreach (var meeting in meetings)
        {
            foreach (var recommendation in meeting.Recommendations)
            {
                if (recommendation.IsVotable)
                {
                    recommendations.Add(recommendation);
                }
            }
        }
        return recommendations;
    }
    
    public IEnumerable<Recommendation> ReadUnvotableRecommendationsByIdWithVotes(int panelId)
    {
        var meetings = dbContext.Meetings.Include(m => m.Recommendations).ThenInclude(r => r.UserVotes).Where(m => m.PanelId == panelId).ToList();
        
        List<Recommendation> recommendations = new List<Recommendation>();
        foreach (var meeting in meetings)
        {
            foreach (var recommendation in meeting.Recommendations)
            {
                if (recommendation.IsVotable)
                {
                    recommendations.Add(recommendation);
                }
            }
        }
        return recommendations;
    }


    public void UpdateRecommendation(Recommendation recommendation)
    {
        dbContext.Recommendations.Update(recommendation);
        dbContext.SaveChanges();
    }

    public IEnumerable<UserVote> ReadUserVotesById(string userId)
    {
        return dbContext.UserVotes.Where(uv => uv.Voter.Id == userId).ToList();
    }

    public void CreateUserVote(UserVote userVote)
    {
        dbContext.UserVotes.Add(userVote);
        dbContext.SaveChanges();
    }

    public void DeleteUserVote(ApplicationUser member, Recommendation recommendation)
    {
        if (recommendation == null)
        {
            throw new ArgumentException($"Aanbeveling bestaat niet.");
        }

        var userVote = dbContext.UserVotes
            .SingleOrDefault(uv => uv.Voter == member && uv.Recommendation == recommendation);

        if (userVote == null)
        {
            throw new InvalidOperationException("Gebruiker heeft niet gestemd op deze aanbeveling.");
        }

        dbContext.UserVotes.Remove(userVote);

        dbContext.SaveChanges();
    }

    public bool DoesUserVoteExist(ApplicationUser member, Recommendation recommendation)
    {
        return dbContext.UserVotes
            .Any(uv => uv.Voter == member && uv.Recommendation == recommendation);
    }

    public IEnumerable<Criteria> ReadExtraCriteriaByPanelId(int panelId)
    {
        var panel = dbContext.Panels
            .Include(p => p.Criteria)
            .ThenInclude(c => c.SubCriteria)
            .FirstOrDefault(p => p.Id == panelId);

        if (panel == null)
            return [];

        return panel.Criteria
            .Where(c => c.Name != "Geslacht" && c.Name != "Leeftijd")
            .ToList();
    }

    public IEnumerable<Criteria> ReadCriteriaByPanelIdWithSubcriteria(int panelId)
    {
        var panel = dbContext.Panels
            .Include(p => p.Criteria)
            .ThenInclude(c => c.SubCriteria)
            .FirstOrDefault(p => p.Id == panelId);

        return panel?.Criteria
            .ToList();
    }

    public IEnumerable<Meeting> ReadMeetingsById(int panelId)
    {
        return dbContext.Meetings
            .Include(m => m.Recommendations)
            .ThenInclude(r => r.UserVotes)
            .Where(m => m.PanelId == panelId).ToList();
    }
}