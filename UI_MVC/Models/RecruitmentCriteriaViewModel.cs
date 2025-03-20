using CitizenPanel.BL.Domain.Draw;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class RecruitmentCriteriaViewModel : IValidatableObject
{
    public int TotalAvailablePotentialPanelmembers { get; set; }
    public double MalePercentage { get; set; }
    public double FemalePercentage { get; set; }
    public double Age18_25Percentage { get; set; }
    public double Age26_40Percentage { get; set; }
    public double Age41_60Percentage { get; set; }
    public double Age60PlusPercentage { get; set; }
    public List<ExtraCriteria> ExtraCriteria { get; set; } = new List<ExtraCriteria>();
    
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MalePercentage + FemalePercentage != 100)
            yield return new ValidationResult("Het totaal van man en vrouw moet precies 100% zijn.");

        if (Age18_25Percentage + Age26_40Percentage + Age41_60Percentage + Age60PlusPercentage != 100)
            yield return new ValidationResult("Het totaal van de leeftijdsgroepen moet precies 100% zijn.");

        foreach (var criteria in ExtraCriteria)
        {
            if (criteria.SubCriteria.Sum(s => s.Percentage) != 100)
                yield return new ValidationResult($"De subcategorieën van '{criteria.Name}' moeten samen 100% zijn.");
        }
    }
}