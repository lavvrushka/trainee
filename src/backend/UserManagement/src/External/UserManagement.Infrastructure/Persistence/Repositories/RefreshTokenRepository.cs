﻿using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.Models;
using UserManagement.Infrastructure.Persistence.Context;

namespace UserManagement.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(UserManagementDbContext context) : base(context) { }
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }
        public async Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(Guid accountId)
        {
            return await _context.Set<RefreshToken>()
                .Where(rt => rt.AccountId == accountId)
                .ToListAsync();
        }
    }
}
