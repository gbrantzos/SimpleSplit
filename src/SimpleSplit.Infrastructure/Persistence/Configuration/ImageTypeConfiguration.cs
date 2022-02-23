using Microsoft.EntityFrameworkCore;
using SimpleSplit.Domain.Features.Common;

namespace SimpleSplit.Infrastructure.Persistence.Configuration
{
    public class ImageTypeConfiguration : EntityTypeConfiguration
    {
        public override void Configure(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Image>().ToTable("image");
            entity.Property(m => m.ID).HasColumnName("id");
            entity.HasKey(m => m.ID);

            entity.Property(m => m.Filename).HasColumnName("filename").HasMaxLength(255).IsRequired();
            entity.Property(m => m.Type).HasColumnName("entity_type").IsRequired();
            entity.Property(m => m.EntityID).HasColumnName("entity_id").IsRequired();
            entity.Property(m => m.Content).HasColumnName("content").IsRequired();

            entity.HasIndex(m => new {m.Type, m.EntityID})
                .IsUnique()
                .HasDatabaseName("IX_image_entity");
        }
    }
}