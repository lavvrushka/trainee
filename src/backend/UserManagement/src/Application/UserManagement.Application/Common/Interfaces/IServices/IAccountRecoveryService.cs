
namespace UserManagement.Application.Common.Interfaces.IServices
{
    public interface IAccountRecoveryService
    {
        public Task<string> GenerateAccountRecoveryTokenAsync(Guid userId);
        public Task SendAccountRecoveryEmailAsync(Guid userId);
        public Task RecoverAccountAsync(Guid userId, string token);
    }
}
