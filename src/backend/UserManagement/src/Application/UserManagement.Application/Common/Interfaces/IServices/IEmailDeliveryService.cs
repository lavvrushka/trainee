namespace UserManagement.Application.Common.Interfaces.IServices;

public interface IEmailDeliveryService
{
    public Task SendEmailAsync(string to, string subject, string body);
}
