using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.DAL;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

public class PanelManager : IPanelManager
{
    private readonly IPanelRepository _panelRepository;
    private readonly IDrawManager _drawManager;

    public PanelManager(IPanelRepository repository, IDrawManager drawManager) {
        _panelRepository = repository;
        _drawManager = drawManager;
    }
    
    public Panel GetPanelById(int panelId)
    {
        return _panelRepository.ReadPanelById(panelId);
    }
    
    public Panel GetPanelByIdWithMembers(int panelId)
    {
        return _panelRepository.ReadPanelByIdWithMembers(panelId);
    }
    public Panel AddPanel(string name, string description, ICollection<Criteria> criteria, OrganizationProfile organization, int totAvPotMem)
    {
        Panel newPanel = new Panel()
        {
            Name = name,
            Description = description,
            MemberCount = 0,
            Criteria = criteria,
            Organization = organization,
            TotalAvailablePotentialPanelmembers = totAvPotMem
        };
        _panelRepository.CreatePanel(newPanel);
        return newPanel;
    }

    public void EditPanel(Panel panel)
    {
        _panelRepository.UpdatePanel(panel);
    }

    public void RemovePanel(Panel panel)
    {
        _panelRepository.DeletePanel(panel);
    }

    public IEnumerable<RecruitmentBucket> GetTargetBucketsByPanel(Panel panel)
    {
        return _panelRepository.ReadTargetBucketsByPanel(panel);
    }
    

    public Panel GetPanelByIdWithRecommendations(int panelId)
    {
        return _panelRepository.ReadPanelByIdWithRecommendations(panelId);
    }

    public Recommendation GetRecommendationById(int recommendationId)
    {
        return _panelRepository.ReadRecommendationById(recommendationId);
    }
    
    public Recommendation GetRecommendationWithVoters(int recommendationId)
    {
        return _panelRepository.ReadRecommendationWithVotersById(recommendationId);
    }

    public void EditRecommendation(Recommendation recommendation)
    {
        _panelRepository.UpdateRecommendation(recommendation);
    }
    public bool HasUserVotedForRecommendation(ApplicationUser member, Recommendation recommendation)
    {
        return _panelRepository.HasUserVotedForRecommendation(member, recommendation);
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
        
        _panelRepository.CreateVoteToRecommendation(userVote);
        
        // Verhoog de stemteller in de aanbeveling
        recommendation.Votes++;
        _panelRepository.UpdateRecommendation(recommendation);
        
    }

    public void RemoveVoteFromRecommendation(ApplicationUser member, Recommendation recommendation)
    {
        _panelRepository.DeleteVoteFromRecommendation(member, recommendation);
    }
    
    public IEnumerable<int> GetVotedRecommendationsByUser(string userId)
    {
        return _panelRepository.ReadVotedRecommendationsByUser(userId);
    }

    public void EditCriteria(Criteria criteria)
    {
        _panelRepository.UpdateCriteria(criteria);
    }
    
    public IEnumerable<Criteria> GetExtraCriteriaByPanelId(int panelId)
    {
        return _panelRepository.ReadExtraCriteriaByPanelId(panelId);
    }

    public IEnumerable<Criteria> GetCriteriaAndSubcriteriaWithPanelId(int panelId)
    {
        return _panelRepository.ReadCriteriaAndSubcriteriaWithPanelId(panelId);
    }
    

}