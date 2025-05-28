using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Validation;

public class MinDateTodayAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return date >= today;
        }

        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} mag niet in het verleden liggen.";
    }
}