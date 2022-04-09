using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Buildings;

namespace SimpleSplit.Infrastructure.Persistence.Configuration
{
    public class BuildingTypeConfiguration : EntityTypeConfiguration<Building, BuildingID>
    {
        public BuildingTypeConfiguration(IEntityIDFactory entityIDFactory) : base(entityIDFactory)
        {
        }

        public override void Configure(EntityTypeBuilder<Building> builder)
        {
            base.Configure(builder);

            builder.OwnsOne(b => b.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("address_street")
                    .HasMaxLength(200)
                    .IsRequired();
                address.Property(a => a.Number)
                    .HasColumnName("address_number")
                    .HasMaxLength(100);
                address.Property(a => a.City)
                    .HasColumnName("address_city")
                    .HasMaxLength(100);
                address.Property(a => a.ZipCode)
                    .HasColumnName("address_zipcode")
                    .HasMaxLength(20);
            });
            builder.Property(b => b.Description)
                .HasColumnName("description")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}