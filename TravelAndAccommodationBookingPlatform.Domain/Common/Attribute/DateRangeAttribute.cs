using System.ComponentModel.DataAnnotations;

public class DateRangeAttribute : ValidationAttribute
{
    private readonly string _startDatePropertyName;

    public DateRangeAttribute(string startDatePropertyName)
    {
        _startDatePropertyName = startDatePropertyName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var endDate = value as DateTime?;
        var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);

        if (startDateProperty == null)
            return new ValidationResult($"Unknown property {_startDatePropertyName}");

        var startDate = startDateProperty.GetValue(validationContext.ObjectInstance) as DateTime?;

        var today = DateTime.Today;

        if (startDate.HasValue && startDate.Value.Date < today)
        {
            return new ValidationResult("StartDate cannot be in the past.");
        }

        if (endDate.HasValue && endDate.Value.Date < today)
        {
            return new ValidationResult("EndDate cannot be in the past.");
        }

        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            return new ValidationResult("StartDate must be earlier than or equal to EndDate.");
        }

        return ValidationResult.Success;
    }
}