using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.DAL.Panels;

namespace CitizenPanel.BL.Panels;

public class PanelManager(
    IPanelRepository repository,
    IDrawManager drawManager) : IPanelManager
{
    private readonly IDrawManager _drawManager = drawManager;

    public Panel GetPanelById(int panelId)
    {
        return repository.ReadPanelById(panelId);
    }
    
    public Panel GetPanelByIdWithInvitations(int panelId)
    {
        return repository.ReadInvitationsOfPanelById(panelId);
    }
    
    public IEnumerable<Invitation> GetRegisteredInvitationsByPanelId(int panelId)
    {
        return repository.ReadRegisteredInvitationsByPanelId(panelId);
    }
    
    public Panel AddPanel(string name, string description, ICollection<Criteria> criteria, int totAvPotMem)
    {
        Panel newPanel = new Panel()
        {
            Name = name,
            Description = description,
            MemberCount = 0,
            Criteria = criteria,
            TotalAvailablePotentialPanelmembers = totAvPotMem
        };
        repository.CreatePanel(newPanel);
        return newPanel;
    }

    public void ChangePanel(Panel panel)
    {
        repository.UpdatePanel(panel);
    }

    public void RemovePanel(Panel panel)
    {
        repository.DeletePanel(panel);
    }

    public IEnumerable<RecruitmentBucket> GetTargetBucketsByPanel(Panel panel)
    {
        return repository.ReadTargetBucketsByPanel(panel);
    }
    

    public Panel GetPanelByIdWithRecommendations(int panelId)
    {
        return repository.ReadPanelByIdWithRecommendations(panelId);
    }

    public Recommendation GetRecommendationById(int recommendationId)
    {
        return repository.ReadRecommendationById(recommendationId);
    }
    
    public Recommendation GetRecommendationWithVoters(int recommendationId)
    {
        return repository.ReadRecommendationWithVotersById(recommendationId);
    }

    public void EditRecommendation(Recommendation recommendation)
    {
        repository.UpdateRecommendation(recommendation);
    }
    public bool HasUserVotedForRecommendation(ApplicationUser member, Recommendation recommendation)
    {
        return repository.HasUserVotedForRecommendation(member, recommendation);
    }

    public void AddVoteToRecommendation(ApplicationUser member, Recommendation recommendation, bool recommended)
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
            VotedAt = DateTime.UtcNow,
            Recommended = recommended,
            TenantId = member.MemberProfile.TenantId
        };
        
        repository.CreateVoteToRecommendation(userVote);
        
        // Verhoog de stemteller in de aanbeveling
        recommendation.Votes++;
        repository.UpdateRecommendation(recommendation);
        
    }

    public void RemoveVoteFromRecommendation(ApplicationUser member, Recommendation recommendation)
    {
        repository.DeleteVoteFromRecommendation(member, recommendation);
    }
    
    public IEnumerable<int> GetVotedRecommendationsByUser(string userId)
    {
        return repository.ReadVotedRecommendationsByUser(userId);
    }

    public void EditCriteria(Criteria criteria)
    {
        repository.UpdateCriteria(criteria);
    }
    
    public IEnumerable<Criteria> GetExtraCriteriaByPanelId(int panelId)
    {
        return repository.ReadExtraCriteriaByPanelId(panelId);
    }

    public IEnumerable<Criteria> GetCriteriaAndSubcriteriaWithPanelId(int panelId)
    {
        return repository.ReadCriteriaAndSubcriteriaWithPanelId(panelId);
    }
    
    public void ChangeRecommendation(Recommendation recommendation)
    {
        repository.UpdateRecommendation(recommendation);
    }

    public IEnumerable<Panel> GetAllPanels()
    {
        return repository.ReadAllPanels();
    }
}