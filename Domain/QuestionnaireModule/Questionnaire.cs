using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.QuestionnaireModule;

public class Questionnaire
{
    public int Id { get; init; }
    
    [Required]
    [MaxLength(1000)]
    public string Title { get; set; }
    
    [MinCount(1)]
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}