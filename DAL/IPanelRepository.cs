using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

using BL.Domain.Recruitment;

public interface IPanelRepository
{
    public ExtraCriteria ReadExtraCriteria(int criteriaId);
    
    public List<ExtraCriteria> ReadAllExtraCriteria();
    
    public SubCriteria ReadSubCriteria(int subCriteriaId);
}