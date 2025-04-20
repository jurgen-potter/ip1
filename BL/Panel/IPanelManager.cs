using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.BL;

public interface IPanelManager
{
    
    public Panel AddPanel(string name, string description, DateOnly endDate, ICollection<Criteria> criteria);

    public Panel GetPanelById(int panelId);
    void AddPanel(Panel panel);
    void EditPanel(Panel panel);
    void RemovePanel(Panel panel);
    public IEnumerable<RecruitmentBucket> GetTargetBucketsByPanel(Panel panel);

}