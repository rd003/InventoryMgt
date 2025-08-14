using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Data.Models;


public class Sale : BaseSchema
{

    [Required]
    public int ProductId { get; set; }

    [Required]
    public double Quantity { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public DateTime SellingDate { get; set; }

    [MaxLength(100)]
    public string? Description { get; set; }

    public string? ProductName { get; set; }
}

public class PaginatedSale
{
    public IEnumerable<Sale> Sales { get; set; } = Enumerable.Empty<Sale>();
    public PaginationBase? Pagination { get; set; }
}