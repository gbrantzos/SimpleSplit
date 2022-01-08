using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Infrastructure.Persistence.Configuration
{
    public static class ConfigurationExtensions
    {
        public static PropertyBuilder<TProperty> IsEntityID<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder,
            string columnName = SimpleSplitDbContext.ID)
            where TProperty : EntityID, new()
        {
            var converter = new ValueConverter<TProperty, long>(
                v => v.IDValue,
                v => EntityID.FromValue<TProperty>(v));
            propertyBuilder
                .HasColumnName(columnName)
                .HasConversion(converter);
            return propertyBuilder;
        }

        public static EntityTypeBuilder<TEntity> AddCommonColumn<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : Entity
        {
            builder.Property(e => e.RowVersion).HasColumnName(SimpleSplitDbContext.RowVersion);
            builder.Property<DateTime>(SimpleSplitDbContext.CreatedAt);
            builder.Property<DateTime>(SimpleSplitDbContext.ModifiedAt);

            return builder;
        }

        public static EntityTypeBuilder<TEntity> HasEntityID<TEntity, TID>(
            this EntityTypeBuilder<TEntity> entityBuilder,
            Expression<Func<TEntity, TID>> propertyExpression,
            string columnName = SimpleSplitDbContext.ID)
            where TEntity : Entity<TID>
            where TID : EntityID, new()
        {
            entityBuilder.Property(propertyExpression).IsEntityID(columnName);
            return entityBuilder;
        }
    }
}
