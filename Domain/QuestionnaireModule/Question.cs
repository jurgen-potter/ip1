using CitizenPanel.BL.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.QuestionnaireModule;

public class Question
{
    public int Id { get; init; }
    
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; }
    
    [Range(1, 10)]
    public int Weight { get; set; }
    
    public int Position { get; set; }
    
    public Questionnaire Questionnaire { get; set; }
    
    public bool IsDetail { get; set; } = false;
    
    [MinCount(1)]
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}