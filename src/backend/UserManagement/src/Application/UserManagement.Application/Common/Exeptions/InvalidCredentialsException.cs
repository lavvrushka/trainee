namespace UserManagement.Application.Common.Exeptions
{
    public class InvalidCredentialsException(string message = "") : Exception(message) { }
}
