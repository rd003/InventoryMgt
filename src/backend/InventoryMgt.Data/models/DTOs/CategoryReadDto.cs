namespace InventoryMgt.Data.Models.DTOs;

public class CategoryReadDto
{
    public int Id { get; set; }
    public string? CategoryName { get; set; }
    public int? CategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
}