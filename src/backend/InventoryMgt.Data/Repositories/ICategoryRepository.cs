using InventoryMgt.Data.Models;
using InventoryMgt.Data.Models.DTOs;

namespace InventoryMgt.Data.Repositories;

public interface ICategoryRepository
{
    Task<CategoryReadDto> AddCategory(Category category);
    Task<CategoryReadDto> UpdateCategory(Category category);
    Task DeleteCategory(int id);
    Task<CategoryReadDto?> GetCategory(int id);
    Task<IEnumerable<CategoryReadDto>> GetCategories(string searchTerm = "");
}