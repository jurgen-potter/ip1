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
}