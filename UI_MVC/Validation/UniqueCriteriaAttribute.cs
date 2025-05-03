using CitizenPanel.UI.MVC.Models;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Validation;

public class UniqueCriteriaAttribute : ValidationAttribute
{
    public UniqueCriteriaAttribute()
    {
        ErrorMessage = "Duplicate criteria names are not allowed.";
    }

    public override bool IsValid(object? value)
    {
        if (value is List<CriteriaViewModel> criteriaList)
        {
            var duplicates = criteriaList
                .GroupBy(c => c.Name?.ToLowerInvariant())
                .Where(g => !string.IsNullOrEmpty(g.Key) && g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            
            if (duplicates.Any())
            {
                ErrorMessage = $"De volgende criteria komen meerdere keren voor: {string.Join(", ", duplicates)}";
                return false;
            }
            
            return true;
        }

        return false;
    }
}