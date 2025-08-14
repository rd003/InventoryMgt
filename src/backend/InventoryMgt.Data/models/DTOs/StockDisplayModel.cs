namespace InventoryMgt.Data.Models.DTOs;

using InventoryMgt.Data.Models;

public class StockDisplayModel : Stock
{
    public string? CategoryName { get; set; }
    public string? ProductName { get; set; }
}

public class PaginatedStock
{
    public IEnumerable<StockDisplayModel> Stocks { get; set; }
    public PaginationBase Pagination { get; set; }
}