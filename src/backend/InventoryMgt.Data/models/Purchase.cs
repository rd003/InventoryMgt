using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Data.Models;
public class Purchase : BaseSchema
{
    [Required]
    public int ProductId { get; set; }
    [Required]
    public double Quantity { get; set; }
    [Required]
    public DateTime PurchaseDate { get; set; }
    [MaxLength(100)]
    public string? Description { get; set; }

    [Required]
    public double Price { get; set; }
    public string? ProductName { get; set; }
}

public class PaginatedPurchase
{
    public IEnumerable<Purchase> Purchases { get; set; }
    public PaginationBase Pagination { get; set; }
}