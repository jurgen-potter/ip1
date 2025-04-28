using CitizenPanel.BL.Domain.Draw;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class RecruitmentCriteriaViewModel
{
    public int PanelId { get; set; }
    
    [Range(10, int.MaxValue, ErrorMessage = "Geef het aantal mensen die mee kunnen doen op.")]
    public int TotalAvailablePotentialPanelmembers { get; set; }
    
    [Required(ErrorMessage = "Er moet minstens één criterium zijn.")]
    public List<CriteriaViewModel> Criteria { get; set; } = new();
}

public class CriteriaViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Geef een naam op voor het criterium.")]
    public string Name { get; set; } // bv. “Gender”, “Age”, “Transport”
    
    [Required(ErrorMessage = "Voeg minstens één subcriterium toe.")]
    public List<SubCriteriaViewModel> SubCriteria { get; set; } = new();
}

public class SubCriteriaViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Geef een naam op voor het subcriterium.")]
    public string Name { get; set; } // bv. “Male”, “Female” of “18-25”, “Bus”, etc.
    
    [Range(0, 100, ErrorMessage = "Percentage moet tussen 0 en 100 zijn.")]
    public double Percentage { get; set; }
}

