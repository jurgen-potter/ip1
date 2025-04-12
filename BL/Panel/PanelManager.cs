using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.BL;

using DAL;

public class PanelManager : IPanelManager
{
    private readonly IPanelRepository _panelRepository;

    public PanelManager(IPanelRepository repository) {
        _panelRepository = repository;
    }
    
    public Panel GetPanelById(int panelId)
    {
        return _panelRepository.ReadPanelById(panelId);
    }

    public void AddPanel(Panel panel)
    {
        _panelRepository.CreatePanel(panel);
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
    
    public void AddRecommendationOfPanel(Recommendation recommendation, Panel panel)
    {
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

    public void EditRecommendation(Recommendation recommendation)
    {
        _panelRepository.UpdateRecommendation(recommendation);
    }
    
    public bool HasUserVotedForRecommendation(string userId, int recommendationId)
    {
        return _panelRepository.HasUserVotedForRecommendation(userId, recommendationId);
    }

    public void AddVoteToRecommendation(string userId, int recommendationId)
    {
        _panelRepository.CreateVoteToRecommendation(userId, recommendationId);
    }

    public void RemoveVoteFromRecommendation(string userId, int recommendationId)
    {
        _panelRepository.DeleteVoteFromRecommendation(userId, recommendationId);
    }
    
    public IEnumerable<int> GetVotedRecommendationIdsByUser(string userId)
    {
        return _panelRepository.ReadVotedRecommendationIdsByUser(userId);
    }
}