using AppointmentsManagement.Domain.Models;
using AppointmentsManagement.Infrastructure.Persistense.Configurations;
using Microsoft.EntityFrameworkCore;
namespace AppointmentsManagement.Infrastructure.Persistense.Context;

public class AppointmentsManagementDbContext : DbContext
{
    public AppointmentsManagementDbContext(DbContextOptions<AppointmentsManagementDbContext> options) : base(options) { }

    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<Result> Results { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
        modelBuilder.ApplyConfiguration(new ResultConfiguration());
    }
}
