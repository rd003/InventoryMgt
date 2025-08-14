namespace InventoryMgt.Data.Models.DTOs;
public class ProductWithStock
{
    public int Id { get; set; }
    public string? ProductName { get; set; }
    public string? CategoryName { get; set; }
    public double Price { get; set; }
    public double Quantity { get; set; }
}

