using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.BL;

public interface IPanelManager
{
    public Panel GetPanelById(int panelId);
    void AddPanel(Panel panel);
    void EditPanel(Panel panel);
    void RemovePanel(Panel panel);
    public IEnumerable<RecruitmentBucket> GetTargetBucketsByPanel(Panel panel);
    void AddRecommendationOfPanel(string title, string description, Panel panel);
    
    public Panel GetPanelByIdWithRecommendations(int panelId);
    public Recommendation GetRecommendationById(int recommendationId);

    public void EditRecommendation(Recommendation recommendation);
    
    bool HasUserVotedForRecommendation(string userId, int recommendationId);
    void AddVoteToRecommendation(string userId, int recommendationId);
    void RemoveVoteFromRecommendation(string userId, int recommendationId);
    IEnumerable<int> GetVotedRecommendationIdsByUser(string userId);

}