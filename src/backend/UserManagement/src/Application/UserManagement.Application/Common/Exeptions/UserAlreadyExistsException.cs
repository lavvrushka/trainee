namespace UserManagement.Application.Common.Exeptions
{
    public class UserAlreadyExistsException(string message = "") : Exception(message) { }
}
