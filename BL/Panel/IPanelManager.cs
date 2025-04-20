using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.BL;

public interface IPanelManager
{
    public Panel GetPanel(int panelId);
    
    public Panel AddPanel(string name, string description, DateOnly endDate, ICollection<ExtraCriteria> criteria);
}