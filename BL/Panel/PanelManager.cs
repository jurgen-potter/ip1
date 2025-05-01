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
    public Panel GetPanelByIdWithoutTenant(int panelId)
    {
        return _panelRepository.ReadPanelByIdWithoutTenant(panelId);
    }

    public Panel AddPanel(string name, string description, ICollection<Criteria> criteria)
    {
        Panel newPanel = new Panel()
        {
            Name = name,
            Description = description,
            MemberCount = 0,
            Criteria = criteria
        };
        _panelRepository.CreatePanel(newPanel);
        //_drawManager.GenerateInvitations(newPanel);
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
    
    public void AddRecommendationOfPanel(string title, string description, Panel panel)
    {
        var recommendation = new Recommendation(title, description, 0);
        _panelRepository.CreateRecommendationOfPanel(recommendation, panel);
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

    public void AddVoteToRecommendation(ApplicationUser member, Recommendation recommendation)
    {
        _panelRepository.CreateVoteToRecommendation(member, recommendation);
    }

    public void RemoveVoteFromRecommendation(ApplicationUser member, Recommendation recommendation)
    {
        _panelRepository.DeleteVoteFromRecommendation(member, recommendation);
    }
    
    public IEnumerable<int> GetVotedRecommendationsByUser(string userId)
    {
        return _panelRepository.ReadVotedRecommendationsByUser(userId);
    }
}