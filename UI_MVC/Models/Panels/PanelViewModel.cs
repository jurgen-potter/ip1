using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models.Panels;

public class PanelViewModel
{
    public int PanelId { get; set; }
    
    public int PanelPartcipants { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public string CoverImagePath { get; set; }
    public string BannerImagePath { get; set; }
    
    
    public List<string> PublicDocumentNames { get; set; } = new();

    public List<MeetingViewModel> Meetings { get; set; } = new();
    public List<RecommendationViewModel> Recommendations { get; set; } = new();
    public List<PostViewModel> Posts { get; set; } = new();
    public bool CanManagePanel { get; set; }
    public bool ShowRejected { get; set; }
}


public class RecommendationViewModel
{
    public int    Id          { get; set; }
    public string Title       { get; set; }
    public string Description { get; set; }
    public bool IsDone { get; set; }
}

public class MeetingViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateOnly Date { get; set; }
    public List<RecommendationViewModel> Recommendations { get; set; } = new();
    public List<RecommendationViewModel> RejectedRecommendations { get; set; } = new();
    public List<string> DocumentNames { get; set; } = new();
    
}

