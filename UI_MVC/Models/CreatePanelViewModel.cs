using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class CreatePanelViewModel
{
    [Required(ErrorMessage = "Dit veld is verplicht.")]
    public string Name { get; set; }
    public string Description { get; set; }
    
    public ResultViewModel Result { get; set; }
}