using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

using BL.Domain.Recruitment;
using Microsoft.EntityFrameworkCore;

public class PanelRepository : IPanelRepository
{
    private readonly PanelDbContext _context;

    public PanelRepository(PanelDbContext context)
    {
        _context = context;
    }

    public ExtraCriteria ReadExtraCriteria(int criteriaId)
    {
        return _context.ExtraCriteria
            .Where(e => e.Id == criteriaId)
            .Include(e => e.SubCriteria)
            .SingleOrDefault();
    }
    public List<ExtraCriteria> ReadAllExtraCriteria()
    {
        return _context.ExtraCriteria
            .Include(e => e.SubCriteria)
            .ToList();
    }

    public SubCriteria ReadSubCriteria(int subCriteriaId)
    {
        return _context.SubCriteria.Find(subCriteriaId);
    }
}