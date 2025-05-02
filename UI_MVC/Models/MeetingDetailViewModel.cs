namespace CitizenPanel.UI.MVC.Models;

public class MeetingDetailViewModel
{
    public int PanelId { get; set; }
    
    public string PanelName { get; set; }
    
    public DateOnly MeetingDate { get; set; }
    
    public List<RecommendationViewModel> Recommendations { get; set; } = new();
}