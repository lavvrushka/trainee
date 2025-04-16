using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Interfaces.Models;
using UserManagement.Infrastructure.Persistence.Configurations;

namespace UserManagement.Infrastructure.Persistence.Context
{
    public class UserManagementDbContext : IdentityDbContext<Account, Role, Guid>
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RefreshTokenConfiguration());
            builder.ApplyConfigurationsFromAssembly(typeof(UserManagementDbContext).Assembly);
        }
    }
}
