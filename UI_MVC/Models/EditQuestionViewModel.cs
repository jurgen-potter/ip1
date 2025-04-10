using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class EditQuestionViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; }
    
    [Range(1, 10)]
    public int Weight { get; set; }

    public List<EditAnswerViewModel> Answers { get; set; } = new();
}