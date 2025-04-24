namespace OfficesManagement.Core.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; set; }
    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occured")
        => Errors = errors;
}