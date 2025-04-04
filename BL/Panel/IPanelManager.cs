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
    void addRecommendationOfPanel(Recommendation recommendation, Panel panel);
    
    public Panel getPanelByIdWithRecommendations(int panelId);
    public Recommendation GetRecommendationById(int recommendationId);

    public void editRecommendation(Recommendation recommendation);
}