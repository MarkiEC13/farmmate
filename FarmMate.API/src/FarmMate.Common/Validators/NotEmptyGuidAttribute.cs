using System.ComponentModel.DataAnnotations;

namespace FarmMate.Common.Validators;

public class NotEmptyGuidAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? guidValue, ValidationContext validationContext) => 
        guidValue is Guid value && Guid.Empty == value ? new ValidationResult("Guid cannot be empty.") : null;
}
