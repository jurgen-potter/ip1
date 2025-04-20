using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.DAL;

namespace CitizenPanel.BL;

public class PanelManager : IPanelManager
{
    private readonly IPanelRepository _panelRepository;
    private readonly IDrawManager _drawManager;

    public PanelManager(IPanelRepository repository, IDrawManager drawManager) {
        _panelRepository = repository;
        _drawManager = drawManager;
    }
    
    public Panel GetPanelById(int panelId)
    {
        return _panelRepository.ReadPanelById(panelId);
    }

    public Panel AddPanel(string name, string description, DateOnly endDate, ICollection<ExtraCriteria> criteria)
    {
        Panel newPanel = new Panel()
        {
            Name = name,
            Description = description,
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = endDate,
            MemberCount = 0,
            ExtraCriteria = criteria
        };
        _panelRepository.CreatePanel(newPanel);
        //_drawManager.GenerateInvitations(newPanel);
        return newPanel;
    }
}