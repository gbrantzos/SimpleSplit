using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Buildings;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Infrastructure.Persistence.Configuration
{
    public class ApartmentTypeConfiguration : EntityTypeConfiguration<Apartment, ApartmentID>
    {
        public ApartmentTypeConfiguration(IEntityIDFactory entityIDFactory) : base(entityIDFactory) { }

        public override void Configure(EntityTypeBuilder<Apartment> builder)
        {
            base.Configure(builder);

            builder.Property(a => a.Code)
                .HasColumnName("code")
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(a => a.SortingNum)
                .HasColumnName("sorting_num")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(a => a.Owner)
                .HasColumnName("owner")
                .HasMaxLength(200)
                .IsRequired();
            builder.Property(a => a.Dweller)
                .HasColumnName("dweller")
                .HasMaxLength(200);
            builder.Property(a => a.BuildingID)
                .HasColumnName("building_id")
                .IsRequired();
            builder.Property(a => a.Ratios)
                .HasConversion<RatiosConverter>()
                .HasColumnName("ratios")
                .HasMaxLength(120)
                .IsRequired();
        }
    }

    public class RatiosConverter : ValueConverter<Dictionary<Category.CategoryKind, double>, string>
    {
        public RatiosConverter() : base(x => ToDatabaseValue(x), x => FromDatabaseValue(x)) { }

        private static string ToDatabaseValue(Dictionary<Category.CategoryKind, double> dictionary)
        {
            var temp = dictionary.Select(kv => $"{(int)kv.Key}:{kv.Value}").ToArray();
            return String.Join("|", temp);
        }

        private static Dictionary<Category.CategoryKind, double> FromDatabaseValue(string value)
        {
            var toReturn = Enum.GetValues<Category.CategoryKind>()
                .ToDictionary(e => e, _ => 0d);
            value.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .ForEach(v =>
                {
                    var temp = v.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (temp.Length == 2)
                    {
                        var kind = Int32.Parse(temp[0]);
                        toReturn[(Category.CategoryKind)kind] = Double.Parse(temp[1]);
                    }
                });
            return toReturn;
        }
    }
}
