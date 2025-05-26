using Microsoft.EntityFrameworkCore;
namespace ProfilesManagement.Infrastructure.Persistence.Context;

public class ProfileManagementDbContext : DbContext
{
    public ProfileManagementDbContext(DbContextOptions<ProfileManagementDbContext> options) : base(options) { }

    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Receptionist> Receptionists { get; set; } = null!;
    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}