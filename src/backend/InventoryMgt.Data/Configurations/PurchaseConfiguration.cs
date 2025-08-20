using InventoryMgt.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMgt.Data.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(e => e.Id).HasName("purchase_pkey");

        builder.ToTable("purchase");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("create_date");
        builder.Property(e => e.Description)
            .HasMaxLength(100)
            .HasColumnName("description");
        builder.Property(e => e.InvoiceNumber)
            .HasMaxLength(50)
            .HasColumnName("invoice_number");
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasColumnName("is_deleted");
        builder.Property(e => e.ProductId).HasColumnName("product_id");
        builder.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
        builder.Property(e => e.PurchaseOrderNumber)
            .HasMaxLength(50)
            .HasColumnName("purchase_order_number");
        builder.Property(e => e.Quantity)
            .HasPrecision(10, 3)
            .HasColumnName("quantity");
        builder.Property(e => e.ReceivedDate).HasColumnName("received_date");
        builder.Property(e => e.SupplierId).HasColumnName("supplier_id");
        builder.Property(e => e.UnitPrice)
            .HasPrecision(18, 2)
            .HasColumnName("unit_price");
        builder.Property(e => e.UpdateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("update_date");

        builder.HasOne(d => d.Product).WithMany(p => p.Purchases)
            .HasForeignKey(d => d.ProductId)
            .HasConstraintName("purchase_product_id_fkey");

        builder.HasOne(d => d.Supplier).WithMany(p => p.Purchases)
            .HasForeignKey(d => d.SupplierId)
            .HasConstraintName("purchase_supplier_id_fkey");
    }
}
