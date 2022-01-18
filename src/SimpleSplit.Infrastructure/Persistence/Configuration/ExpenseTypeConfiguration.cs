using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Infrastructure.Persistence.Configuration
{
    public class ExpenseTypeConfiguration : EntityTypeConfiguration<Expense, ExpenseID>
    {
        public ExpenseTypeConfiguration(IEntityIDFactory entityIDFactory) : base(entityIDFactory) { }

        public override void Configure(EntityTypeBuilder<Expense> builder)
        {
            base.Configure(builder);

            var value = builder.OwnsOne(m => m.Amount);
            value.Property(v => v.Amount).HasColumnName("amount");
            value.Property(v => v.Currency).HasColumnName("amount_currency");

            builder.Property(m => m.Description).HasColumnName("description");
            builder.Property(m => m.EnteredAt).HasColumnName("entered_at");
            builder.HasOne(m => m.Category)
                .WithMany()
                .HasForeignKey("category_id");
            builder.Property(m => m.IsOwnerCharge).HasColumnName("is_owner_charge");
            builder.Property(m => m.SharedAt).HasColumnName("shared_at");
        }
    }
}
