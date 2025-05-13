using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Validation;

public class MinCountAttribute : ValidationAttribute
{
    private readonly int _minCount;

    public MinCountAttribute(int minCount)
    {
        _minCount = minCount;
        ErrorMessage = $"At least {_minCount} item(s) required.";
    }

    public override bool IsValid(object value)
    {
        if (value is IList list)
        {
            return list.Count >= _minCount;
        }

        return false;
    }
}