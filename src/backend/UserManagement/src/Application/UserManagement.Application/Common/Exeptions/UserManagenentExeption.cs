namespace UserManagement.Application.Common.Exeptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message = "") : base(message) { }
}

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message = "") : base(message) { }
}

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string message = "") : base(message) { }
}

public class ValidationAppException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; set; }

    public ValidationAppException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
}
