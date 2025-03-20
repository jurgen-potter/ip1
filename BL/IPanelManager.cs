using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.BL;

public interface IPanelManager
{
    public Panel GetPanel(int panelId);
}