namespace UserManagement.Application.Common.Interfaces.IServices
{
    public interface IPasswordResetService
    {
        public Task<string> GeneratePasswordResetTokenAsync(Guid userId);
        public Task SendPasswordResetEmailAsync(Guid userId);
        public Task<bool> ResetPasswordAsync(Guid userId, string token, string newPassword);
    }
}

