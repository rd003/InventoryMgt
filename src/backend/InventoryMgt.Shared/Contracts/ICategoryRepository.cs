using InventoryMgt.Data.models.DTOs;
using InventoryMgt.Data.Models.DTOs;

namespace InventoryMgt.Data.Repositories;

public interface ICategoryRepository
{
    Task<CategoryReadDto> AddCategory(CategoryDto category);
    Task<CategoryReadDto> UpdateCategory(CategoryDto category);
    Task DeleteCategory(int id);
    Task<CategoryReadDto?> GetCategory(int id);
    Task<IEnumerable<CategoryReadDto>> GetCategories(string searchTerm = "");
}