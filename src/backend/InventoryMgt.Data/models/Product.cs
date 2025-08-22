namespace InventoryMgt.Data.Models;

public class Product
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public int? SupplierId { get; set; }

    public string Sku { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual Stock? Stock { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
