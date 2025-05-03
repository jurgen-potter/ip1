using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.QuestionnaireModule;

public class Answer
{
    public int Id { get; init; }
    
    [Required]
    [MaxLength(1000)]
    public string Description  { get; set; }
    
    //[Required]
    [MaxLength(1000)]
    public string Advice { get; set; }
    
    public bool IsCritical{ get; set; }
    
    public int Position { get; set; }
    
    public Question Question { get; set; }
}