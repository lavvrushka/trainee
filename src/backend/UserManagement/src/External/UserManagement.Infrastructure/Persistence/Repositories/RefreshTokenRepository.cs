using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.Models;
using UserManagement.Infrastructure.Persistence.Context;
namespace UserManagement.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(UserManagementDbContext context) : base(context) { }
    public Task<RefreshToken?> GetByTokenAndUserIdAsync(string token, Guid userId)
    {
        return _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == token && rt.AccountId == userId);
    }

    public Task<List<RefreshToken>> GetAllByUserIdAsync(Guid accountId)
    {
        var result = _context.Set<RefreshToken>()
            .Where(rt => rt.AccountId == accountId)
            .ToList();  

        return Task.FromResult(result);
    }
}
