using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models.Panels;

public class CreatePostViewModel
{
    [Required(ErrorMessage = "De titel van de post is verplicht.")]
    [StringLength(50, ErrorMessage = "De titel mag maximaal 50 tekens lang zijn.")]
    [Display(Name = "Titel")]
    public string Title { get; set; }

    [Required(ErrorMessage = "De inhoud van de post is verplicht.")]
    [StringLength(200, ErrorMessage = "De inhoud mag maximaal 200 tekens lang zijn.")]
    [Display(Name = "Inhoud")]
    public string Content { get; set; }
    public int PanelId { get; set; }
}