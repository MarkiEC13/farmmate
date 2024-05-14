using System.Diagnostics.CodeAnalysis;

namespace FarmMate.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class NotFoundException : Exception
{
    public NotFoundException() : base() { }

    public NotFoundException(string message) : base(message) { }
}
