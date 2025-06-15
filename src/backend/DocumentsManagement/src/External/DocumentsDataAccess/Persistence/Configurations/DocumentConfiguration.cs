using DocumentsDataAccess.Persistence.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
namespace DocumentsDataAccess.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<DocumentEntity>
{
    public void Configure(EntityTypeBuilder<DocumentEntity> builder)
    {
        builder.ToTable("documents");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
               .HasColumnName("id")
               .IsRequired();

        builder.Property(d => d.BlobUrl)
               .HasColumnName("blob_url")
               .IsRequired();

        builder.Property(d => d.LastRetrievedAt)
               .HasColumnName("last_retrieved_at")
               .IsRequired();

        builder.Property(d => d.IsDeleted)
               .HasColumnName("is_deleted")
               .IsRequired();
    }
}
