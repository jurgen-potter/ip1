using System.ComponentModel.DataAnnotations;
using CitizenPanel.UI.MVC.Models.Draws;

namespace CitizenPanel.UI.MVC.Models.Panels;

public class CreatePanelViewModel
{
    [Required(ErrorMessage = "Dit veld is verplicht.")]
    public string Name { get; set; }
    public string Description { get; set; }
    
    public ResultViewModel Result { get; set; }
}