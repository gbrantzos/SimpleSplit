using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Infrastructure.Persistence.Configuration
{
    public class ExpenseTypeConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder
                .ToTable("expense")
                .HasEntityID(e => e.ID)
                .AddCommonColumn();

            var value = builder.OwnsOne(m => m.Amount);
            value.Property(v => v.Amount).HasColumnName("amount");
            value.Property(v => v.Currency).HasColumnName("amount_currency");

            builder.Property(m => m.Description).HasColumnName("description");
            builder.Property(m => m.EnteredAt).HasColumnName("entered_at");
            builder.Property(m => m.Category).HasColumnName("category");
            builder.Property(m => m.IsOwnerCharge).HasColumnName("is_owner_charge");
            builder.Property(m => m.SharedAt).HasColumnName("shared_at");
        }
    }
}
