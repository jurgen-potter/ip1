using CitizenPanel.BL.Domain.Recruitment;

namespace CitizenPanel.BL;

public interface IPanelManager
{
    public ExtraCriteria GetExtraCriteria(int criteriaId);

    public List<ExtraCriteria> GetAllExtraCriteria();

    public SubCriteria GetSubCriteria(int subCriteriaId);
}