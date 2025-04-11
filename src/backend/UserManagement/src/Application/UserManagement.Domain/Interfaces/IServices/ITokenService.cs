using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Domain.Interfaces.IServices
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(Account account);
        Task<string> GenerateRefreshTokenAsync(Guid accountId);
        Task RevokeRefreshTokenAsync();
        Task<(string newAccessToken, string RefreshToken)> RefreshTokensAsync(string token);
        Guid? ExtractUserIdFromToken(string token);
        string? ExtractTokenFromHeader();
        Task<Account> AuthenticateUserAsync(string token);
    }
}
