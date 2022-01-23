using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Infrastructure.Persistence.Configuration;

public class UserTypeConfiguration : EntityTypeConfiguration<User, UserID>
{
    public UserTypeConfiguration(IEntityIDFactory entityIDFactory) : base(entityIDFactory) { }

    public override void Configure(EntityTypeBuilder<User> builder)
    {
        ConfigureBase(builder, "users");
        builder.HasIndex(m => m.Username).IsUnique();

        builder.Property(m => m.Username)
            .HasColumnName("user_name")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(m => m.DisplayName)
            .HasColumnName("display_name");
        builder.Property(m => m.Email)
            .HasColumnName("email")
            .HasMaxLength(320);
        builder.Property(m => m.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(100);
        builder.Property(m => m.PasswordSalt)
            .HasColumnName("password_salt")
            .HasMaxLength(100);
    }
}