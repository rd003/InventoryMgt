using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Data.Models;

public class Stock
{
    public int Id { get; set; }
    [Required]
    public int ProductId { get; set; }
    [Required]
    public decimal Quantity { get; set; }
}
