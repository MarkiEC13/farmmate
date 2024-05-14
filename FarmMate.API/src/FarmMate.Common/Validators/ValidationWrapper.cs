using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FarmMate.Common.Validators;

[ExcludeFromCodeCoverage]
public class ValidationWrapper<T>
{
    public bool IsValid { get; set; }
    public T Value { get; set; }

    public IEnumerable<ValidationResult> ValidationResults { get; set; }
}
