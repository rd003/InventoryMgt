using InventoryMgt.Data.models.DTOs;

namespace InventoryMgt.Data.Repositories;

public interface ISaleRepository
{
    Task<SaleReadDto> AddSale(SaleDto sale);
    Task<SaleReadDto> UpdateSale(SaleDto sale);
    Task RemoveSale(int id);
    Task<SaleReadDto?> GetSale(int id);
    Task<PaginatedSale> GetSales(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null);

}