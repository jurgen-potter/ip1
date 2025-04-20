using CitizenPanel.BL.Domain.Draw;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class CreatePanelViewModel
{
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public DateOnly EndDate { get; set; }
}