using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Shared.Repositories;

public interface IProductRepository
{
    Task<ProductDisplay> AddProduct(ProductDto product);
    Task<ProductDisplay> UpdatProduct(ProductDto product);
    Task DeleteProduct(int id);
    Task<PagedProduct> GetProducts(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null);
    Task<ProductDisplay?> GetProduct(int id);
    Task<IEnumerable<ProductWithStock>> GetAllProductsWithStock();
}