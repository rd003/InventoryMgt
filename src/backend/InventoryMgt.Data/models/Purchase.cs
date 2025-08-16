using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Data.Models;

public class Purchase
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }
    [Required]
    public double Quantity { get; set; }
    [Required]
    public DateTime PurchaseDate { get; set; }
    [MaxLength(100)]
    public string? Description { get; set; }
    [Required]
    public double UnitPrice { get; set; }
    [MaxLength(50)]
    public string? PurchaseOrderNumber { get; set; }
    [MaxLength(50)]
    public string? InvoiceNumber { get; set; }
    public DateTime? ReceivedDate { get; set; }
}

public class PurchaseReadDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public double Quantity { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
}

public class PaginatedPurchase
{
    public IEnumerable<PurchaseReadDto> Purchases { get; set; }
    public PaginationBase Pagination { get; set; }
}