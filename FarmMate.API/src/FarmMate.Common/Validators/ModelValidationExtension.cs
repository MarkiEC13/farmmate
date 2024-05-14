using System.ComponentModel.DataAnnotations;

namespace FarmMate.Common.Validators;

public static class ModelValidationExtension
{
    public static ValidationWrapper<T> Validate<T>(this T model) where T : class
    {
        var validator = new ValidationWrapper<T>
        {
            Value = model
        };

        var results = new List<ValidationResult>();
        validator.IsValid = Validator.TryValidateObject(validator.Value,
            new ValidationContext(validator.Value, null, null),
            results, true);
        validator.ValidationResults = results;

        return validator;
    }
}
