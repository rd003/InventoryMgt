using System;
using InventoryMgt.Data.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMgt.Data.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(e => e.Id).HasName("stock_pkey");

        builder.ToTable("stock");

        builder.HasIndex(e => e.IsDeleted, "idx_stock_active").HasFilter("(is_deleted = false)");

        builder.HasIndex(e => e.Quantity, "idx_stock_quantity");

        builder.HasIndex(e => e.ProductId, "stock_product_id_key").IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("create_date");
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasColumnName("is_deleted");
        builder.Property(e => e.ProductId).HasColumnName("product_id");
        builder.Property(e => e.Quantity)
            .HasPrecision(10, 3)
            .HasColumnName("quantity");
        builder.Property(e => e.UpdateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("update_date");

        builder.HasOne(d => d.Product).WithOne(p => p.Stock)
            .HasForeignKey<Stock>(d => d.ProductId)
            .HasConstraintName("stock_product_id_fkey");
    }
}
