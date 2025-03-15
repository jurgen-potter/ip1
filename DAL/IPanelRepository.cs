using CitizenPanel.BL.Domain.PanelManagement;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

public interface IPanelRepository
{
    Panel GetPanelById(int panelId);
    IEnumerable<Panel> GetAllPanels();
    void AddPanel(Panel panel);
    void UpdatePanel(Panel panel);
    void DeletePanel(Panel panel);
}