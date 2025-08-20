using InventoryMgt.Data.Models.DTOs;

namespace InventoryMgt.Data.Repositories;

public interface IStockRepository
{
    Task<PaginatedStock> GetStocks(int page = 1, int limit = 4, string sortColumn = "Id", string sortDirection = "asc", string? searchTerm = null);
    Task<StockDisplayModel?> GetStockByProductId(int productId);
}