using System.ComponentModel.DataAnnotations;
using CitizenPanel.UI.MVC.Validation;

namespace CitizenPanel.UI.MVC.Models.Draws;

public class RecruitmentCriteriaViewModel
{
    public int PanelId { get; set; }
    
    [Range(100, int.MaxValue, ErrorMessage = "Er moet een pool zijn van meer dan 100 mensen.")]
    public int TotalAvailablePotentialPanelmembers { get; set; }
    
    [Required(ErrorMessage = "Er moet minstens één criterium zijn.")]
    [UniqueCriteria]
    public List<CriteriaViewModel> Criteria { get; set; } = new();
}

