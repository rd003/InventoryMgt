using InventoryMgt.Data.Models;
using InventoryMgt.Data.Models.DTOs;

namespace InventoryMgt.Data.Repositories;

public interface IProductRepository
{
    Task<ProductDisplay> AddProduct(Product product);
    Task<ProductDisplay> UpdatProduct(Product product);
    Task DeleteProduct(int id);
    Task<PagedProduct> GetProducts(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null);
    Task<ProductDisplay?> GetProduct(int id);
    Task<IEnumerable<ProductWithStock>> GetAllProductsWithStock();
}