using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Data.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string? CategoryName { get; set; }
    public int? CategoryId { get; set; }
}