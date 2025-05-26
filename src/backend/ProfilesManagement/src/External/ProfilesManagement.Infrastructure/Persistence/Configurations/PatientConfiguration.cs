using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
namespace ProfilesManagement.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.MiddleName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.AccountId).IsRequired();

        builder.HasOne(p => p.Image)
               .WithMany()
               .HasForeignKey(p => p.ImageId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
