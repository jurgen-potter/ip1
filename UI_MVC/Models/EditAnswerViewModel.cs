using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class EditAnswerViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; }
}