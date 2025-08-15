namespace InventoryMgt.Data.Models.DTOs;

public class ProductDisplay
{
    public int Id { get; set; }
    public string? ProductName { get; set; }
    public double Price { get; set; }
    public string? CategoryName { get; set; }
    public string? SupplierName { get; set; }
}