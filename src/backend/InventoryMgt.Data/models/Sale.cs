using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Data.Models;


public class Sale
{

    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public decimal Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public DateTime SellingDate { get; set; }

    [MaxLength(100)]
    public string? Description { get; set; }
}

public class SaleReadDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public DateTime SellingDate { get; set; }

    public string? Description { get; set; }

    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
}

public class PaginatedSale
{
    public IEnumerable<SaleReadDto> Sales { get; set; } = Enumerable.Empty<SaleReadDto>();
    public PaginationBase? Pagination { get; set; }
}