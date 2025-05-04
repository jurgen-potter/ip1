using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

public interface IPanelManager
{
    
    public Panel AddPanel(string name, string description, ICollection<Criteria> criteria, OrganizationProfile organization, int totAvPotMem);

    public Panel GetPanelById(int panelId);
    Panel GetPanelByIdWithMembers(int panelId);
    void EditPanel(Panel panel);
    void RemovePanel(Panel panel);

    public Panel GetPanelById(int panelId);
    public void ChangePanel(Panel panel);
    public void RemovePanel(Panel panel);
    public IEnumerable<RecruitmentBucket> GetTargetBucketsByPanel(Panel panel);
    
    public Panel GetPanelByIdWithRecommendations(int panelId);
    public Recommendation GetRecommendationById(int recommendationId);
    public Recommendation GetRecommendationWithVoters(int recommendationId);

    
    public void EditRecommendation(Recommendation recommendation);
    
    public bool HasUserVotedForRecommendation(ApplicationUser member, Recommendation recommendation);
    public void AddVoteToRecommendation(ApplicationUser member, Recommendation recommendation, bool recommended);
    public void RemoveVoteFromRecommendation(ApplicationUser member, Recommendation recommendation);
    public IEnumerable<int> GetVotedRecommendationsByUser(string userId);
    public void EditCriteria(Criteria criteria);
    public IEnumerable<Criteria> GetExtraCriteriaByPanelId(int panelId);
    IEnumerable<Criteria> GetCriteriaAndSubcriteriaWithPanelId(int panelId);
    public void ChangeRecommendation(Recommendation recommendation);
    public IEnumerable<Panel> GetAllPanels();
}