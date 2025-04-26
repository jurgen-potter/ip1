using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

public interface IPanelManager
{
    
    public Panel AddPanel(string name, string description, DateOnly endDate, ICollection<Criteria> criteria);

    public Panel GetPanelById(int panelId);
    void EditPanel(Panel panel);
    void RemovePanel(Panel panel);
    public IEnumerable<RecruitmentBucket> GetTargetBucketsByPanel(Panel panel);
    void AddRecommendationOfPanel(string title, string description, Panel panel);
    
    public Panel GetPanelByIdWithRecommendations(int panelId);
    public Recommendation GetRecommendationById(int recommendationId);
    public Recommendation GetRecommendationWithVoters(int recommendationId);


    public void EditRecommendation(Recommendation recommendation);
    
    bool HasUserVotedForRecommendation(ApplicationUser member, Recommendation recommendation);
    void AddVoteToRecommendation(ApplicationUser member, Recommendation recommendation);
    void RemoveVoteFromRecommendation(ApplicationUser member, Recommendation recommendation);
    IEnumerable<int> GetVotedRecommendationsByUser(string userId);

}