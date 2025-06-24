using AppointmentsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
namespace AppointmentsManagement.Infrastructure.Persistense.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<Result>
{
    public void Configure(EntityTypeBuilder<Result> builder)
    {
        builder.ToTable("Results");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Complaints)
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.Property(r => r.Conclusion)
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.Property(r => r.Recommendations)
               .HasMaxLength(1000)
               .IsRequired(false);

        builder
            .HasOne(r => r.Appointment)
            .WithOne(a => a.Result)
            .HasForeignKey<Result>(r => r.AppointmentId);
    }
}
