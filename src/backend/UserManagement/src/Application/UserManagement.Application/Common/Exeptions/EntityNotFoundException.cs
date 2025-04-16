namespace UserManagement.Application.Common.Exeptions
{
    public class EntityNotFoundException(string message = "") : Exception(message) { }
}
