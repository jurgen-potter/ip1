namespace CitizenPanel.BL;

using DAL;
using Domain.Recruitment;

public class PanelManager : IPanelManager
{
    private readonly IPanelRepository _repository;

    public PanelManager(IPanelRepository repository) {
        _repository = repository;
    }

    public ExtraCriteria GetExtraCriteria(int criteriaId)
    {
        return _repository.ReadExtraCriteria(criteriaId);
    }
    
    public List<ExtraCriteria> GetAllExtraCriteria()
    {
        return _repository.ReadAllExtraCriteria();
    }
    
    public SubCriteria GetSubCriteria(int subCriteriaId)
    {
        return _repository.ReadSubCriteria(subCriteriaId);
    }
}