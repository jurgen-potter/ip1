using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models.DTO;

public class EditPanelDto
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    public DateOnly EndDate { get; set; }
    
    public string CoverImagePath { get; set; }
}