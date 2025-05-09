using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.DAL.Panels;

public interface IPanelRepository
{
    Panel ReadPanelById(int panelId);
    Panel ReadInvitationsOfPanelById(int panelId);
    IEnumerable<Invitation>ReadRegisteredInvitationsByPanelId(int panelId);

    void CreatePanel(Panel panel);
    void UpdatePanel(Panel panel);
    void DeletePanel(Panel panel);
    public IEnumerable<RecruitmentBucket> ReadTargetBucketsByPanel(Panel panel);
    Panel ReadPanelByIdWithRecommendations(int panelId);
    Recommendation ReadRecommendationById(int recommendationId);
    public Recommendation ReadRecommendationWithVotersById(int recommendationId);
    public void UpdateRecommendation(Recommendation recommendation);
    
    bool HasUserVotedForRecommendation(ApplicationUser member, Recommendation recommendation);
    void CreateVoteToRecommendation(UserVote userVote);
    void DeleteVoteFromRecommendation(ApplicationUser member, Recommendation recommendation);
    
    IEnumerable<int> ReadVotedRecommendationsByUser(string userId);
    void UpdateCriteria(Criteria criteria);
    IEnumerable<Criteria> ReadExtraCriteriaByPanelId(int panelId);
    IEnumerable<Criteria> ReadCriteriaAndSubcriteriaWithPanelId(int panelId);
    IEnumerable<Panel> ReadAllPanels();
}