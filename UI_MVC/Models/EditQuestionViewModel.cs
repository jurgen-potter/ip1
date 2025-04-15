using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class EditQuestionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vraag mag niet leeg zijn.")]
    [MaxLength(1000, ErrorMessage = "Vraag mag maximaal 1000 karakters bevatten.")]
    public string Description { get; set; }
    
    [Range(1, 10)]
    public int Weight { get; set; }

    public List<EditAnswerViewModel> Answers { get; set; } = new();
}