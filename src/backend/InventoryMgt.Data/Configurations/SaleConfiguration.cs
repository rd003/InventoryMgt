using InventoryMgt.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMgt.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasKey(e => e.Id).HasName("sale_pkey");

        builder.ToTable("sale");

        builder.HasIndex(e => e.IsDeleted, "idx_sale_active").HasFilter("(is_deleted = false)");

        builder.HasIndex(e => e.SellingDate, "idx_sale_date");

        builder.HasIndex(e => new { e.SellingDate, e.ProductId }, "idx_sale_date_product").HasFilter("(is_deleted = false)");

        builder.HasIndex(e => e.ProductId, "idx_sale_product");

        builder.HasIndex(e => new { e.ProductId, e.SellingDate }, "idx_sale_product_date");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("create_date");
        builder.Property(e => e.Description)
            .HasMaxLength(100)
            .HasColumnName("description");
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasColumnName("is_deleted");
        builder.Property(e => e.Price)
            .HasPrecision(18, 2)
            .HasColumnName("price");
        builder.Property(e => e.ProductId).HasColumnName("product_id");
        builder.Property(e => e.Quantity)
            .HasPrecision(10, 3)
            .HasColumnName("quantity");
        builder.Property(e => e.SellingDate).HasColumnName("selling_date");
        builder.Property(e => e.UpdateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("update_date");

        builder.HasOne(d => d.Product).WithMany(p => p.Sales)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("sale_product_id_fkey");
    }
}
