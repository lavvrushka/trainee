using DocumentsDataAccess.Persistence.Configurations;
using DocumentsDataAccess.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentsDataAccess.Persistence.Context;

public class AppDbContext : DbContext
{
    public DbSet<DocumentEntity> Documents { get; set; }
    public DbSet<ImageEntity> Images { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new DocumentConfiguration());
        builder.ApplyConfiguration(new ImageConfiguration());
        base.OnModelCreating(builder);
    }
}
