using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;

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
    public Recommendation ReadRecommendationWithVotersById(int recommendationId);
    public void UpdateRecommendation(Recommendation recommendation);
    
    bool HasUserVotedForRecommendation(ApplicationUser member, Recommendation recommendation);
    void CreateVoteToRecommendation(ApplicationUser member, Recommendation recommendation);
    void DeleteVoteFromRecommendation(ApplicationUser member, Recommendation recommendation);
    
    IEnumerable<int> ReadVotedRecommendationsByUser(string userId);
    void UpdateCriteria(Criteria criteria);
    IEnumerable<Criteria> ReadExtraCriteriaByPanelId(int panelId);
}