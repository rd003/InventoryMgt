using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Shared.Repositories;

public interface ISaleRepository
{
    Task<SaleReadDto> AddSale(SaleDto sale);
    Task<SaleReadDto> UpdateSale(SaleDto sale);
    Task RemoveSale(int id);
    Task<SaleReadDto?> GetSale(int id);
    Task<PaginatedSale> GetSales(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null);

}