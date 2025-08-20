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
