using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models.Draws;

public class CriteriaViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Geef een naam op voor het criteria.")]
    public string Name { get; set; } // bv. “Gender”, “Age”, “Transport”
    
    [Required(ErrorMessage = "Voeg minstens één subcriteria toe.")]
    public List<SubCriteriaViewModel> SubCriteria { get; set; } = new();
}

public class SubCriteriaViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Geef een naam op voor het subcriteria.")]
    public string Name { get; set; } // bv. “Male”, “Female” of “18-24”, “Bus”, etc.
    
    [Range(0, 100, ErrorMessage = "Percentage moet tussen 0 en 100 zijn.")]
    public double Percentage { get; set; }
}