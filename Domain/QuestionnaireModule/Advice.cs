using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.QuestionnaireModule;

public class Advice
{
    public int Id { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    public int MinScore { get; set; }
    
    public int MaxScore { get; set; }
    
    public ICollection<string> Links { get; set; }
}