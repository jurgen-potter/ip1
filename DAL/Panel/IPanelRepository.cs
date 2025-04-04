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
}