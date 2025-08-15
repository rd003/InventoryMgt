using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace InventoryMgt.Data.Models;

public class Category
{
    public int Id { get; set; }
    [NotNull]
    [MaxLength(50)]
    public string? CategoryName { get; set; }
    public int? CategoryId { get; set; }
}