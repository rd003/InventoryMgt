using InventoryMgt.Data.Models;

namespace InventoryMgt.Data.Repositories;

public interface ISaleRepository
{
    Task<SaleReadDto> AddSale(Sale sale);
    Task<SaleReadDto> UpdateSale(Sale sale);
    Task RemoveSale(int id);
    Task<SaleReadDto?> GetSale(int id);
    Task<PaginatedSale> GetSales(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null);

}