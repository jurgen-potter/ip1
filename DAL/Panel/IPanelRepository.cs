using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.DAL;

public interface IPanelRepository
{
    Panel ReadPanelById(int panelId);
    void CreatePanel(Panel panel);
    void UpdatePanel(Panel panel);
    void DeletePanel(Panel panel);
    public IEnumerable<RecruitmentBucket> ReadTargetBucketsByPanel(Panel panel);
    public void CreateRecommendationOfPanel(Recommendation recommendation, Panel panel);
    Panel ReadPanelByIdWithRecommendations(int panelId);
    Recommendation ReadRecommendationById(int recommendationId);
    public void UpdateRecommendation(Recommendation recommendation);
    
    bool HasUserVotedForRecommendation(string userId, int recommendationId);
    void CreateVoteToRecommendation(string userId, int recommendationId);
    void DeleteVoteFromRecommendation(string userId, int recommendationId);
    
    IEnumerable<int> ReadVotedRecommendationIdsByUser(string userId);
}