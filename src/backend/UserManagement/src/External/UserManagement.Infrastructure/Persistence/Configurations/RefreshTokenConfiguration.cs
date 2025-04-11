using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(rt => rt.Expires)
                .IsRequired();
            builder.Property(rt => rt.Created)
                .IsRequired();
            builder.HasOne(rt => rt.Account)
                .WithMany()
                .HasForeignKey(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(rt => rt.AccountId);
            builder.HasIndex(rt => rt.Expires);
        }
    }
}
