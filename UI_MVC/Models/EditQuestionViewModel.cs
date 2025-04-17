using CitizenPanel.BL.Domain.QuestionnaireModule;
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

    [MinCount(1, ErrorMessage = "Er moet minstens een antwoord aangemaakt zijn.")]
    public List<EditAnswerViewModel> Answers { get; set; } = new();
    
    public bool ToDelete { get; set; }
}