using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models.Panels;

public class AddRecommendationViewModel
{
    public int MeetingId { get; set; }
    public int PanelId { get; set; }
    
    [Required(ErrorMessage = "Je moet een titel invullen")]
    public string Title { get; set; }
    
    public string Description { get; set; }
    public bool IsAnonymous { get; set; }
    public double NeededPercentage { get; set; }
}
