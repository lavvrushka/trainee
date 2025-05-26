using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
namespace ProfilesManagement.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.LastName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.MiddleName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.CareerStartYear).IsRequired();

        builder.Property(d => d.Status)
               .HasConversion<string>() 
               .IsRequired();

        builder.Property(d => d.AccountId).IsRequired();

        builder.HasOne(d => d.Image)
               .WithMany()
               .HasForeignKey(d => d.ImageId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
