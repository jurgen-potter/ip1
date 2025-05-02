using System.ComponentModel.DataAnnotations;
using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.UI.MVC.Models;

public class PanelViewModel
{
    public int PanelId { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public string CoverImagePath { get; set; }

    public List<MeetingViewModel> Meetings { get; set; } = new();
    public List<RecommendationViewModel> Recommendations { get; set; } = new();
}

public class RecommendationViewModel
{
    public int    Id          { get; set; }
    public string Title       { get; set; }
    public string Description { get; set; }
}

public class MeetingViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateOnly Date { get; set; }
    public List<RecommendationViewModel> Recommendations { get; set; } = new();
    
}

