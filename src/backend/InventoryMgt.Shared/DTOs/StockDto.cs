using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Shared.DTOs;

public class StockDto
{
    public int Id { get; set; }
    [Required]
    public int ProductId { get; set; }
    [Required]
    public decimal Quantity { get; set; }
}
