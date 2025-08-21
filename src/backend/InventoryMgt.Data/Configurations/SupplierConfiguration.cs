
using InventoryMgt.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMgt.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("supplier");
        builder.HasKey(e => e.Id).HasName("supplier_pkey");
        builder.HasIndex(e => e.IsActive, "idx_supplier_active").HasFilter("(is_active = true)");

        builder.HasIndex(e => e.Email, "idx_supplier_email");

        builder.HasIndex(e => e.SupplierName, "idx_supplier_name");

        builder.HasIndex(e => e.IsDeleted, "idx_supplier_not_deleted").HasFilter("(is_deleted = false)");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Address)
            .HasMaxLength(300)
            .HasColumnName("address");
        builder.Property(e => e.City)
            .HasMaxLength(50)
            .HasColumnName("city");
        builder.Property(e => e.ContactPerson)
            .HasMaxLength(100)
            .HasColumnName("contact_person");
        builder.Property(e => e.Country)
            .HasMaxLength(50)
            .HasColumnName("country");
        builder.Property(e => e.CreateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("create_date");
        builder.Property(e => e.Email)
            .HasMaxLength(100)
            .HasColumnName("email");
        builder.Property(e => e.IsActive)
            .HasDefaultValue(true)
            .HasColumnName("is_active");
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasColumnName("is_deleted");
        builder.Property(e => e.PaymentTerms)
            .HasDefaultValue(30)
            .HasColumnName("payment_terms");
        builder.Property(e => e.Phone)
            .HasMaxLength(20)
            .HasColumnName("phone");
        builder.Property(e => e.PostalCode)
            .HasMaxLength(20)
            .HasColumnName("postal_code");
        builder.Property(e => e.State)
            .HasMaxLength(50)
            .HasColumnName("state");
        builder.Property(e => e.SupplierName)
            .HasMaxLength(100)
            .HasColumnName("supplier_name");
        builder.Property(e => e.TaxNumber)
            .HasMaxLength(50)
            .HasColumnName("tax_number");
        builder.Property(e => e.UpdateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("update_date");
    }
}
