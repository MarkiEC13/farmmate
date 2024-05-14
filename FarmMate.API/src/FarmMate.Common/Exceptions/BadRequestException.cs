using System.Diagnostics.CodeAnalysis;

namespace FarmMate.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class BadRequestException : Exception
{
    public BadRequestException() : base() { }

    public BadRequestException(string message) : base(message) { }
}
