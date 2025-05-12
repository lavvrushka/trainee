using UserManagement.Domain.Interfaces.Models;
namespace UserManagement.Domain.Interfaces.IServices;

public interface ITokenService
{
    public Task<TokensPair> GenerateTokensPairAsync(Account user);
    public Task RevokeRefreshTokenAsync();
    public Task<TokensPair> RefreshTokensAsync(string refreshToken);
    public Guid GetUserIdFromAccessToken();
}
