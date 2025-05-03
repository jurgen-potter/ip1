using System.ComponentModel.DataAnnotations;
using CitizenPanel.BL.Domain.Draw;

namespace CitizenPanel.UI.MVC.Models;

public class RegisterViewModel : IValidatableObject
{
    public string Code { get; set; }
    public List<CriteriaViewModel> CriteriaList { get; set; } = new List<CriteriaViewModel>();
    public List<int> SelectedCriteria { get; set; } = new List<int>();
    public bool IsConfirmed { get; set; }
    
    [Required(ErrorMessage = "Email adres is verplicht")]
    [EmailAddress(ErrorMessage = "Email adres moet geldig zijn")]
    public string Email { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> errors = new List<ValidationResult>();
        if (SelectedCriteria == null || SelectedCriteria.Count < CriteriaList.Count)
        {
            errors.Add(new ValidationResult("Duid voor elke criteria iets aan", new[] { nameof(SelectedCriteria) }));
        }
        return errors;
    }
}