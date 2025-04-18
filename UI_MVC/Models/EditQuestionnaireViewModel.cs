using CitizenPanel.BL.Domain.QuestionnaireModule;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class EditQuestionnaireViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Title { get; set; }

    [MinCount(1, ErrorMessage = "Er moet minstens een vraag aangemaakt zijn.")]
    public List<EditQuestionViewModel> Questions { get; set; } = new();
}