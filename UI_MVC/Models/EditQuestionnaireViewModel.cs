using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class EditQuestionnaireViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Title { get; set; }

    public List<EditQuestionViewModel> Questions { get; set; } = new();
}