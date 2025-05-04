using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.BL.Domain.Validation;
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

public class EditQuestionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vraag mag niet leeg zijn.")]
    [MaxLength(1000, ErrorMessage = "Vraag mag maximaal 1000 karakters bevatten.")]
    public string Description { get; set; }
    
    /*[Range(1, 10)]
    public int Weight { get; set; }*/

    [MinCount(1, ErrorMessage = "Er moet minstens een antwoord aangemaakt zijn.")]
    public List<EditAnswerViewModel> Answers { get; set; } = new();
    
    public bool IsDetail { get; set; }
    
    public bool ToDelete { get; set; }
}

public class EditAnswerViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Antwoord mag niet leeg zijn.")]
    [MaxLength(1000, ErrorMessage = "Antwoord mag maximaal 1000 karakters bevatten.")]
    public string Description { get; set; }
    
    public bool IsCritical{ get; set; }
    
    public bool ToDelete { get; set; }
}