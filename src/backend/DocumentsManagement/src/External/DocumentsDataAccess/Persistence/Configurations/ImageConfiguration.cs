using DocumentsDataAccess.Persistence.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
namespace DocumentsDataAccess.Persistence.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<ImageEntity>
{
    public void Configure(EntityTypeBuilder<ImageEntity> builder)
    {
        builder.ToTable("images");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
               .HasColumnName("id")
               .IsRequired();

        builder.Property(i => i.BlobUrl)
               .HasColumnName("blob_url")
               .IsRequired();

        builder.Property(i => i.LastRetrievedAt)
               .HasColumnName("last_retrieved_at")
               .IsRequired();

        builder.Property(i => i.IsDeleted)
               .HasColumnName("is_deleted")
               .IsRequired();
    }
}
