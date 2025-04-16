using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Domain.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        public Task<RefreshToken?> GetByTokenAndUserIdAsync(string token, Guid userId);
        public Task<List<RefreshToken>> GetAllByUserIdAsync(Guid userId);
    }

}
