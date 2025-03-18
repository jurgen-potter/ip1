using CitizenPanel.BL.Domain.Recruitment;
using CitizenPanel.BL.Domain.PanelManagement;

namespace CitizenPanel.BL;

public interface IPanelManager
{
    public ExtraCriteria GetExtraCriteria(int criteriaId);

    public List<ExtraCriteria> GetAllExtraCriteria();

    public SubCriteria GetSubCriteria(int subCriteriaId);
    
    public Panel GetPanel(int panelId);
}