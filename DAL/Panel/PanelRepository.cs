using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using Microsoft.EntityFrameworkCore;
using CitizenPanel.DAL.Data;

namespace CitizenPanel.DAL;

public class PanelRepository : IPanelRepository
{
    private readonly PanelDbContext _dbContext;

    public PanelRepository(PanelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Panel ReadPanelById(int panelId)
    {
        return _dbContext.Panels
            .Include(p => p.DrawResult)
            .ThenInclude(dr => dr.SelectedMembers)
            .ThenInclude(m => m.MemberProfile)
            .Include(p => p.DrawResult)
            .ThenInclude(dr => dr.ReserveMembers)
            .ThenInclude(m => m.MemberProfile)
            .SingleOrDefault(p => p.PanelId == panelId);
    }

    public Panel ReadPanelByIdWithRecommendations(int panelId)
    {
        return _dbContext.Panels
            .Include(r => r.Recommendations)
            .SingleOrDefault(p => p.PanelId == panelId);
    }


    public void CreatePanel(Panel panel)
    {
        _dbContext.Panels.Add(panel);
        _dbContext.SaveChanges();
    }

    public void UpdatePanel(Panel panel)
    {
        _dbContext.Update(panel);
        _dbContext.SaveChanges();
    }

    public void DeletePanel(Panel panel)
    {
        _dbContext.Panels.Remove(panel);
    }

    public IEnumerable<RecruitmentBucket> ReadTargetBucketsByPanel(Panel panel)
    {
        var panelWithBuckets = _dbContext.Panels
            .Include(p => p.RecruitmentBuckets)
            .FirstOrDefault(p => p.PanelId == panel.PanelId);
        return panelWithBuckets?.RecruitmentBuckets.ToList();
    }

    public void CreateRecommendationOfPanel(Recommendation recommendation, Panel panel)
    {
        panel.Recommendations.Add(recommendation);
        UpdatePanel(panel);
    }

    public Recommendation ReadRecommendationById(int recommendationId)
    {
        return _dbContext.Recommendations
            .Find(recommendationId);
    }

    public Recommendation ReadRecommendationWithVotersById(int recommendationId)
    {
        return _dbContext.Recommendations
            .Include(r => r.UserVotes) 
            .ThenInclude(uv => uv.Voter)
            .ThenInclude(v => v.MemberProfile)
            .SingleOrDefault(r => r.Id == recommendationId);
    }
    
    public void UpdateRecommendation(Recommendation recommendation)
    {
        _dbContext.Recommendations.Update(recommendation);
        _dbContext.SaveChanges();
    }

    public bool HasUserVotedForRecommendation(ApplicationUser member, Recommendation recommendation)
    {
        return _dbContext.UserVotes
            .Any(uv => uv.Voter == member && uv.Recommendation == recommendation);
    }

    public void CreateVoteToRecommendation(ApplicationUser member, Recommendation recommendation)
    {
        // Controleer eerst of de aanbeveling bestaat
        if (recommendation == null)
        {
            throw new ArgumentException($"Aanbeveling met bestaat niet.");
        }

        // Controleer of gebruiker al heeft gestemd
        if (HasUserVotedForRecommendation(member, recommendation))
        {
            throw new InvalidOperationException("Gebruiker heeft al gestemd op deze aanbeveling.");
        }
        // Creëer een nieuwe stem
        var userVote = new UserVote
        {
            Voter = member,
            Recommendation = recommendation,
            VotedAt = DateTime.UtcNow
        };

        // Voeg de stem toe aan de database
        _dbContext.UserVotes.Add(userVote);

        // Verhoog de stemteller in de aanbeveling
        recommendation.Votes++;

        _dbContext.SaveChanges();
    }

    public void DeleteVoteFromRecommendation(ApplicationUser member, Recommendation recommendation)
    {
        // Controleer eerst of de aanbeveling bestaat
        if (recommendation == null)
        {
            throw new ArgumentException($"Aanbeveling bestaat niet.");
        }

        // Zoek de stem van de gebruiker
        var userVote = _dbContext.UserVotes
            .FirstOrDefault(uv => uv.Voter == member && uv.Recommendation == recommendation);

        // Als er geen stem is, kan er niets worden teruggetrokken
        if (userVote == null)
        {
            throw new InvalidOperationException("Gebruiker heeft niet gestemd op deze aanbeveling.");
        }

        // Verwijder de stem
        _dbContext.UserVotes.Remove(userVote);

        // Verlaag de stemteller in de aanbeveling (voorkom negatieve stemmen)
        if (recommendation.Votes > 0)
        {
            recommendation.Votes--;
        }

        _dbContext.SaveChanges();
    }

    public IEnumerable<int> ReadVotedRecommendationsByUser(string userId)
    {
        return _dbContext.UserVotes
            .Where(uv => uv.Voter.Id == userId)
            .Select(uv => uv.RecommendationId)
            .ToList();
    }
}