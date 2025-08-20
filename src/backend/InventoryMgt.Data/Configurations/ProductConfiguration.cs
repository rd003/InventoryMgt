using InventoryMgt.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMgt.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.Id).HasName("product_pkey");

        builder.ToTable("product");

        builder.HasIndex(e => e.IsDeleted, "idx_product_active").HasFilter("(is_deleted = false)");

        builder.HasIndex(e => e.CategoryId, "idx_product_category");

        builder.HasIndex(e => new
        {
            e.CategoryId,
            e.IsDeleted
        }, "idx_product_category_active");

        builder.HasIndex(e => e.ProductName, "idx_product_name");

        builder.HasIndex(e => e.Price, "idx_product_price");

        builder.HasIndex(e => e.SupplierId, "idx_product_supplier");

        builder.HasIndex(e => new { e.SupplierId, e.IsDeleted }, "idx_product_supplier_active");

        builder.HasIndex(e => e.Sku, "product_sku_key").IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CategoryId).HasColumnName("category_id");
        builder.Property(e => e.CreateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("create_date");
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasColumnName("is_deleted");
        builder.Property(e => e.Price)
            .HasPrecision(18, 2)
            .HasColumnName("price");
        builder.Property(e => e.ProductName)
            .HasMaxLength(50)
            .HasColumnName("product_name");
        builder.Property(e => e.Sku)
            .HasMaxLength(100)
            .HasColumnName("sku");
        builder.Property(e => e.SupplierId).HasColumnName("supplier_id");
        builder.Property(e => e.UpdateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("update_date");

        builder.HasOne(d => d.Category).WithMany(p => p.Products)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("product_category_id_fkey");

        builder.HasOne(d => d.Supplier).WithMany(p => p.Products)
            .HasForeignKey(d => d.SupplierId)
            .HasConstraintName("product_supplier_id_fkey");
    }
}