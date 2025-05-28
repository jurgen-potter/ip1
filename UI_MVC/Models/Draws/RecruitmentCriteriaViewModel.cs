using System.ComponentModel.DataAnnotations;
using CitizenPanel.UI.MVC.Validation;

namespace CitizenPanel.UI.MVC.Models.Draws;

public class RecruitmentCriteriaViewModel
{
    public int PanelId { get; set; }
    
    [Range(minimum: 100, maximum: 1000000, ErrorMessage = "Er moet een pool zijn van 100 tot 1.000.000 mensen.")]
    public int TotalAvailablePotentialPanelmembers { get; set; }
    
    [Required(ErrorMessage = "Er moet minstens één criterium zijn.")]
    [UniqueCriteria]
    public List<CriteriaViewModel> Criteria { get; set; } = new();
}

