using System.ComponentModel.DataAnnotations;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.BL.Domain.Questionnaires;

public class Answer
{
    public int Id { get; init; }
    
    [Required]
    [MaxLength(1000)]
    public string Description  { get; set; }
    
    [MaxLength(1000)]
    public string Advice { get; set; }
    
    public bool IsCritical{ get; set; }
    
    public int Position { get; set; }
    
    public Question Question { get; set; }
    
    public ICollection<OrganizationProfile> OrganizationProfiles { get; set; }
}