using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class FinalDrawViewModel
{
    public int PanelId { get; set; }
    [Required]
    public string SelectedSubject { get; set; }
    [Required]
    public string SelectedMessage { get; set; }
    [Required]
    public string ReserveSubject { get; set; }
    [Required]
    public string ReserveMessage { get; set; }
}