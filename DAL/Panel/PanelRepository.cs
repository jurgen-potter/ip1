using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
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
            .Include(p => p.DrawResult)
            .ThenInclude(dr => dr.ReserveMembers)
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

    public void UpdateRecommendation(Recommendation recommendation)
    {
        _dbContext.Recommendations.Update(recommendation);
        _dbContext.SaveChanges();
    }

    public bool HasUserVotedForRecommendation(string userId, int recommendationId)
    {
        return _dbContext.UserVotes
            .Any(uv => uv.UserId == userId && uv.RecommendationId == recommendationId);
    }

    public void CreateVoteToRecommendation(string userId, int recommendationId)
    {
        // Controleer eerst of de aanbeveling bestaat
        var recommendation = ReadRecommendationById(recommendationId);
        if (recommendation == null)
        {
            throw new ArgumentException($"Aanbeveling met ID {recommendationId} bestaat niet.");
        }

        // Controleer of gebruiker al heeft gestemd
        if (HasUserVotedForRecommendation(userId, recommendationId))
        {
            throw new InvalidOperationException("Gebruiker heeft al gestemd op deze aanbeveling.");
        }

        // Creëer een nieuwe stem
        var userVote = new UserVote
        {
            UserId = userId,
            RecommendationId = recommendationId,
            VotedAt = DateTime.UtcNow
        };

        // Voeg de stem toe aan de database
        _dbContext.UserVotes.Add(userVote);

        // Verhoog de stemteller in de aanbeveling
        recommendation.Votes++;

        _dbContext.SaveChanges();
    }

    public void DeleteVoteFromRecommendation(string userId, int recommendationId)
    {
        // Controleer eerst of de aanbeveling bestaat
        var recommendation = ReadRecommendationById(recommendationId);
        if (recommendation == null)
        {
            throw new ArgumentException($"Aanbeveling met ID {recommendationId} bestaat niet.");
        }

        // Zoek de stem van de gebruiker
        var userVote = _dbContext.UserVotes
            .FirstOrDefault(uv => uv.UserId == userId && uv.RecommendationId == recommendationId);

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

    public IEnumerable<int> ReadVotedRecommendationIdsByUser(string userId)
    {
        return _dbContext.UserVotes
            .Where(uv => uv.UserId == userId)
            .Select(uv => uv.RecommendationId)
            .ToList();
    }
}