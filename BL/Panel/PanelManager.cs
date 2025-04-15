using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.BL;

using DAL;

public class PanelManager : IPanelManager
{
    private readonly IPanelRepository _panelRepository;

    public PanelManager(IPanelRepository repository) {
        _panelRepository = repository;
    }
    
    public Panel GetPanel(int panelId)
    {
        return _panelRepository.ReadPanelById(panelId);
    }

    public Panel AddPanel(string name, string description, DateOnly endDate)
    {
        Panel newPanel = new Panel()
        {
            Name = name,
            Description = description,
            EndDate = endDate,
            MemberCount = 0
        };
        _panelRepository.CreatePanel(newPanel);
        return newPanel;
    }
}