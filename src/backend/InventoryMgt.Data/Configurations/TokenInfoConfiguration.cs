using InventoryMgt.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMgt.Data.Configurations;

public class TokenInfoConfiguration : IEntityTypeConfiguration<TokenInfo>
{
    public void Configure(EntityTypeBuilder<TokenInfo> builder)
    {
        builder.ToTable("token_info");

        builder.HasKey(a => a.Id)
        .HasName("pk_token_info");

        builder.HasIndex(a => a.Username, "uq_token_info_username")
            .IsUnique();

        builder.HasIndex(a => a.RefreshToken, "uq_token_info_refresh_token");

        builder.Property(p => p.Id)
        .HasColumnName("id");

        builder.Property(p => p.Username)
        .HasColumnName("username")
       .IsRequired()
       .HasMaxLength(100);

        builder.Property(p => p.RefreshToken)
        .HasColumnName("refresh_token")
        .HasMaxLength(200);

        builder.Property(p => p.ExpiredAt)
        .IsRequired()
        .HasColumnName("expired_at");
    }
}
