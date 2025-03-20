using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.DAL;

public interface IPanelRepository
{
    Panel ReadPanelById(int panelId);
    IEnumerable<Panel> ReadAllPanels();
    void CreatePanel(Panel panel);
    void UpdatePanel(Panel panel);
    void DeletePanel(Panel panel);
}