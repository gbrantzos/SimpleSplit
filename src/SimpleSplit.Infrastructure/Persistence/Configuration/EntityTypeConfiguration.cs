using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Infrastructure.Persistence.Configuration
{
    // Based on https://github.com/dotnet/efcore/issues/23103#issuecomment-720662870
    public abstract class EntityTypeConfiguration
{
        public abstract void Configure(ModelBuilder modelBuilder);
    }

    public abstract class EntityTypeConfiguration<TEntity, TID> : EntityTypeConfiguration, IEntityTypeConfiguration<TEntity>
        where TEntity : Entity<TID>
        where TID : EntityID, new()
    {
        private readonly IEntityIDFactory _entityIDFactory;

        protected EntityTypeConfiguration(IEntityIDFactory entityIDFactory)
            => _entityIDFactory = entityIDFactory;

        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
            => ConfigureBase(builder);

        public override void Configure(ModelBuilder modelBuilder)
            => Configure(modelBuilder.Entity<TEntity>());

        protected void ConfigureBase(EntityTypeBuilder<TEntity> builder)
            => ConfigureBase(builder, typeof(TEntity).Name.ToLower());

        protected void ConfigureBase(EntityTypeBuilder<TEntity> builder, string entityName)
        {
            builder.ToTable(entityName);
            AddCommonColumn(builder);
            SetEntityID(builder, e => e.ID);
        }

        protected void AddCommonColumn(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(e => e.RowVersion).HasColumnName(SimpleSplitDbContext.RowVersion);
            builder.Property<DateTime>(SimpleSplitDbContext.CreatedAt);
            builder.Property<DateTime>(SimpleSplitDbContext.ModifiedAt);
        }

        protected void SetEntityID(EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TID>> propertyExpression,
            string columnName = SimpleSplitDbContext.ID)
        {
            var converter = new ValueConverter<TID, long>(
                v => v.Value,
                v => _entityIDFactory.GetID<TID>(v));

            builder.Property(propertyExpression)
                .HasColumnName(columnName)
                .HasConversion(converter);

            builder.HasKey(e => e.ID);
            builder.Property(propertyExpression)
                .HasColumnName(columnName)
                .HasConversion(converter);
        }
    }
}
