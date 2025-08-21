using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Shared.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string? ProductName { get; set; }
    [Required]
    public int CategoryId { get; set; }
    [Required]
    public int SupplierId { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required, MaxLength(100)]
    public string Sku { get; set; } = string.Empty;
}