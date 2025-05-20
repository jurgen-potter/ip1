namespace CitizenPanel.UI.MVC.Models.Panels;

public class MeetingDetailViewModel
{
    public int MeetingId { get; set; }
    public int PanelId { get; set; }
    
    public string PanelName { get; set; }
    
    public DateOnly MeetingDate { get; set; }
    
    public List<RecommendationViewModel> Recommendations { get; set; } = new();
    
    public List<string> DocumentNames { get; set; } = new();
}