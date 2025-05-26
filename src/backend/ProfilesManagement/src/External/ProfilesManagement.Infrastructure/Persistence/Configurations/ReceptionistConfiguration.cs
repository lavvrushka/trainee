using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
namespace ProfilesManagement.Infrastructure.Persistence.Configurations;

public class ReceptionistConfiguration : IEntityTypeConfiguration<Receptionist>
{
    public void Configure(EntityTypeBuilder<Receptionist> builder)
    {
        builder.ToTable("Receptionists");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(r => r.LastName).IsRequired().HasMaxLength(100);
        builder.Property(r => r.MiddleName).IsRequired().HasMaxLength(100);

        builder.Property(r => r.Status)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(r => r.AccountId).IsRequired();

        builder.HasOne(r => r.Image)
               .WithMany()
               .HasForeignKey(r => r.ImageId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}