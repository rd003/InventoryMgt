using System.ComponentModel.DataAnnotations;

namespace InventoryMgt.Shared.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string? CategoryName { get; set; }
    public int? CategoryId { get; set; }
}