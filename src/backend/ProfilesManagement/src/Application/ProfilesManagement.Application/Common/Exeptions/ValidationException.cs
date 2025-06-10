namespace ProfilesManagement.Application.Common.Exeptions;

public class ValidationEventAppException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationEventAppException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
}
