using CitizenPanel.BL.Domain.PanelManagement;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

using BL.Domain.Recruitment;

public interface IPanelRepository
{
    Panel ReadPanelById(int panelId);
    IEnumerable<Panel> ReadAllPanels();
    void CreatePanel(Panel panel);
    void UpdatePanel(Panel panel);
    void DeletePanel(Panel panel);
    public ExtraCriteria ReadExtraCriteria(int criteriaId);
    
    public List<ExtraCriteria> ReadAllExtraCriteria();
    
    public SubCriteria ReadSubCriteria(int subCriteriaId);
}