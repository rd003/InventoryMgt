using InventoryMgt.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMgt.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder.HasKey(b => b.Id).HasName("pk_user_id");

        builder.HasIndex(b => b.Username, "uq_username")
               .IsUnique();

        builder.Property(b => b.Id).HasColumnName("id");

        builder.Property(b => b.FullName)
        .HasColumnName("full_name")
        .IsRequired()
        .HasMaxLength(100);

        builder.Property(b => b.Username)
        .HasColumnName("user_name")
        .IsRequired()
        .HasMaxLength(100);

        builder.Property(b => b.PasswordHash)
        .HasColumnName("password_hash")
        .IsRequired()
        .HasMaxLength(200);

        builder.Property(b => b.Role)
        .HasColumnName("role")
        .IsRequired()
        .HasMaxLength(15);
    }
}
