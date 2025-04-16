namespace UserManagement.Domain.Interfaces.IServices
{
    public interface IEmailConfirmationService
    {
        public Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
        public Task SendConfirmationEmailAsync(Guid userId);
        public Task<bool> ConfirmEmailAsync(Guid userId, string token);
    }
}
