using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Infrastructure.Persistence.Configuration
{
    public class CategoryTypeConfiguration : EntityTypeConfiguration<Category, CategoryID>
    {
        public CategoryTypeConfiguration(IEntityIDFactory entityIDFactory) : base(entityIDFactory) { }

        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            base.Configure(builder);

            builder.Property(m => m.Description).HasColumnName("description");
            builder.Property(m => m.Kind).HasColumnName("kind");
        }
    }
}
