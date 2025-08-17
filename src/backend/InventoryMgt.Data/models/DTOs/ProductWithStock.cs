namespace InventoryMgt.Data.Models.DTOs;

public class ProductWithStock
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
}

