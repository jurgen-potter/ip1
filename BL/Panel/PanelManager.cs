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
    

    public Panel AddPanel(string name, string description, DateOnly endDate, ICollection<Criteria> criteria)
    {
        Panel newPanel = new Panel()
        {
            Name = name,
            Description = description,
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = endDate,
            MemberCount = 0,
            Criteria = criteria
        };
        _panelRepository.CreatePanel(newPanel);
        //_drawManager.GenerateInvitations(newPanel);
        return newPanel;
    }
    
    public void AddPanel(Panel panel)
    {
        _panelRepository.CreatePanel(panel);
    }

    public void EditPanel(Panel panel)
    {
        _panelRepository.UpdatePanel(panel);
    }

    public void RemovePanel(Panel panel)
    {
        _panelRepository.DeletePanel(panel);
    }

    public IEnumerable<RecruitmentBucket> GetTargetBucketsByPanel(Panel panel)
    {
        return _panelRepository.ReadTargetBucketsByPanel(panel);
    }
}

