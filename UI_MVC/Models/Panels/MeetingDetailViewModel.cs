namespace CitizenPanel.UI.MVC.Models.Panels;

public class MeetingDetailViewModel
{
    public int MeetingId { get; set; }
    public int PanelId { get; set; }
    
    public string PanelName { get; set; }
    
    public DateOnly MeetingDate { get; set; }
    
    public ICollection<RecommendationViewModel> Recommendations { get; set; }
    
    public List<string> DocumentNames { get; set; } = new();
}