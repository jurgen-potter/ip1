using CitizenPanel.UI.MVC.Validation;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class RecruitmentCriteriaViewModel
{
    public int PanelId { get; set; }
    
    [Range(100, int.MaxValue, ErrorMessage = "Er moeten meer dan 100 mensen mee kunnen doen.")]
    public int TotalAvailablePotentialPanelmembers { get; set; }
    
    [Required(ErrorMessage = "Er moet minstens één criterium zijn.")]
    [UniqueCriteria]
    public List<CriteriaViewModel> Criteria { get; set; } = new();
}

