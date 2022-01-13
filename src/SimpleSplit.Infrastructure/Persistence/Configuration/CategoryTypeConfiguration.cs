using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Infrastructure.Persistence.Configuration
{
    public class CategoryTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder
                .ToTable("category")
                .HasEntityID(e => e.ID)
                .AddCommonColumn();

            builder.Property(m => m.Description).HasColumnName("description");
            builder.Property(m =>m.Kind).HasColumnName("kind");
        }
    }
}
