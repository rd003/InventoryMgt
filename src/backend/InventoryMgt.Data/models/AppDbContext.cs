using InventoryMgt.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InventoryMgt.Data.models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Purchase> Purchases { get; set; }

    public DbSet<Sale> Sales { get; set; }

    public DbSet<Stock> Stocks { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }


}
