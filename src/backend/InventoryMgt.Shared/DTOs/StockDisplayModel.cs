namespace InventoryMgt.Data.Models.DTOs;

using InventoryMgt.Data.models.DTOs;

public class StockDisplayModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
}

public class PaginatedStock
{
    public IEnumerable<StockDisplayModel> Stocks { get; set; } = [];
    public PaginationBase Pagination { get; set; } = null!;
}