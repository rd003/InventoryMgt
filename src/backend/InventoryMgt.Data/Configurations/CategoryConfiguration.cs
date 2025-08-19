using InventoryMgt.Data.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryMgt.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(e => e.Id).HasName("pk_category_id");

        builder.ToTable("category");

        builder.HasIndex(e => e.IsDeleted, "idx_category_active").HasFilter("(is_deleted = false)");

        builder.HasIndex(e => e.CategoryName, "idx_category_name");

        builder.HasIndex(e => e.CategoryId, "idx_category_parent").HasFilter("(category_id IS NOT NULL)");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CategoryId).HasColumnName("category_id");
        builder.Property(e => e.CategoryName)
            .HasMaxLength(50)
            .HasColumnName("category_name");
        builder.Property(e => e.CreateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("create_date");
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasColumnName("is_deleted");
        builder.Property(e => e.UpdateDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("update_date");

        builder.HasOne(d => d.CategoryNavigation).WithMany(p => p.InverseCategoryNavigation)
            .HasForeignKey(d => d.CategoryId)
            .HasConstraintName("fk_category_parent");
    }
}
