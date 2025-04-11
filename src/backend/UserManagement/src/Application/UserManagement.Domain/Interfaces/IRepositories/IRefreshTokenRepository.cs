using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Domain.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(Guid userId);
    }
}
