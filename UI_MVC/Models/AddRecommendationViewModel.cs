using System.ComponentModel.DataAnnotations;
using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.UI.MVC.Models;

public class AddRecommendationViewModel
{
    [Required (ErrorMessage = "Je moet een titel invullen")] 
    public string Title { get; set; }
    public string Description { get; set; }
    
    public Panel Panel { get; set; }
}