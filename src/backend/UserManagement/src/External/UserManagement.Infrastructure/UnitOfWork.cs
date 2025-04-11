using UserManagement.Domain.Interfaces;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Infrastructure.Persistence.Context;
using UserManagement.Infrastructure.Persistence.Repositories;

namespace UserManagement.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManagementDbContext _context;
        private IRefreshTokenRepository? _refreshTokenRepository;
        public UnitOfWork(UserManagementDbContext context)
        {
            _context = context;
        }
        public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository ??= new RefreshTokenRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
