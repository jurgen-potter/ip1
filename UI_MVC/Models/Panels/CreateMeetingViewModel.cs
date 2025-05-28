using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models.Panels;

public class CreateMeetingViewModel : IValidatableObject
{
    
    public int PanelId { get; set; }

    public int PanelPartcipants { get; set; }

    [Required(ErrorMessage = "Vul een titel in")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Selecteer een datum")]
    [DataType(DataType.Date, ErrorMessage = "Ongeldige datumnotatie")]
    public DateOnly Date { get; set; }

    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Date < DateOnly.FromDateTime(DateTime.Now))
        {
            yield return new ValidationResult(
                "De datum mag niet in het verleden liggen.",
                new[] { nameof(Date) });
        }
    }
}