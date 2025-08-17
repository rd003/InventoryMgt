using Microsoft.EntityFrameworkCore;

namespace InventoryMgt.Data.models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public  DbSet<Category> Categories { get; set; }

    public  DbSet<Product> Products { get; set; }

    public  DbSet<Purchase> Purchases { get; set; }

    public  DbSet<Sale> Sales { get; set; }

    public  DbSet<Stock> Stocks { get; set; }

    public  DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_category_id");

            entity.ToTable("category");

            entity.HasIndex(e => e.IsDeleted, "idx_category_active").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.CategoryName, "idx_category_name");

            entity.HasIndex(e => e.CategoryId, "idx_category_parent").HasFilter("(category_id IS NOT NULL)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("update_date");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.InverseCategoryNavigation)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("fk_category_parent");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_pkey");

            entity.ToTable("product");

            entity.HasIndex(e => e.IsDeleted, "idx_product_active").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.CategoryId, "idx_product_category");

            entity.HasIndex(e => new { e.CategoryId, e.IsDeleted }, "idx_product_category_active");

            entity.HasIndex(e => e.ProductName, "idx_product_name");

            entity.HasIndex(e => e.Price, "idx_product_price");

            entity.HasIndex(e => e.SupplierId, "idx_product_supplier");

            entity.HasIndex(e => new { e.SupplierId, e.IsDeleted }, "idx_product_supplier_active");

            entity.HasIndex(e => e.Sku, "product_sku_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Price)
                .HasPrecision(18, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .HasColumnName("product_name");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_category_id_fkey");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("product_supplier_id_fkey");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("purchase_pkey");

            entity.ToTable("purchase");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(50)
                .HasColumnName("invoice_number");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            entity.Property(e => e.PurchaseOrderNumber)
                .HasMaxLength(50)
                .HasColumnName("purchase_order_number");
            entity.Property(e => e.Quantity)
                .HasPrecision(10, 3)
                .HasColumnName("quantity");
            entity.Property(e => e.ReceivedDate).HasColumnName("received_date");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 2)
                .HasColumnName("unit_price");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Product).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("purchase_product_id_fkey");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("purchase_supplier_id_fkey");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sale_pkey");

            entity.ToTable("sale");

            entity.HasIndex(e => e.IsDeleted, "idx_sale_active").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.SellingDate, "idx_sale_date");

            entity.HasIndex(e => new { e.SellingDate, e.ProductId }, "idx_sale_date_product").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.ProductId, "idx_sale_product");

            entity.HasIndex(e => new { e.ProductId, e.SellingDate }, "idx_sale_product_date");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Price)
                .HasPrecision(18, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasPrecision(10, 3)
                .HasColumnName("quantity");
            entity.Property(e => e.SellingDate).HasColumnName("selling_date");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Product).WithMany(p => p.Sales)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sale_product_id_fkey");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("stock_pkey");

            entity.ToTable("stock");

            entity.HasIndex(e => e.IsDeleted, "idx_stock_active").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.Quantity, "idx_stock_quantity");

            entity.HasIndex(e => e.ProductId, "stock_product_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasPrecision(10, 3)
                .HasColumnName("quantity");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Product).WithOne(p => p.Stock)
                .HasForeignKey<Stock>(d => d.ProductId)
                .HasConstraintName("stock_product_id_fkey");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("supplier_pkey");

            entity.ToTable("supplier");

            entity.HasIndex(e => e.IsActive, "idx_supplier_active").HasFilter("(is_active = true)");

            entity.HasIndex(e => e.Email, "idx_supplier_email");

            entity.HasIndex(e => e.SupplierName, "idx_supplier_name");

            entity.HasIndex(e => e.IsDeleted, "idx_supplier_not_deleted").HasFilter("(is_deleted = false)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("contact_person");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.PaymentTerms)
                .HasDefaultValue(30)
                .HasColumnName("payment_terms");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("state");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(100)
                .HasColumnName("supplier_name");
            entity.Property(e => e.TaxNumber)
                .HasMaxLength(50)
                .HasColumnName("tax_number");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("update_date");
        });

        base.OnModelCreating(modelBuilder);
    }

   
}
