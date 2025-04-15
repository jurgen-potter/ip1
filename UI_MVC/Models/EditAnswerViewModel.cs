using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class EditAnswerViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Antwoord mag niet leeg zijn.")]
    [MaxLength(1000, ErrorMessage = "Antwoord mag maximaal 1000 karakters bevatten.")]
    public string Description { get; set; }
    
    public bool IsCritical{ get; set; }
}