using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Shared.Repositories;

public interface IPurchaseRepository
{
    Task<PurchaseDto> AddPurchase(PurchaseDto purchase);
    Task<PurchaseDto> UpdatePurchase(PurchaseDto purchase);
    Task RemovePurchase(int id);
    Task<PurchaseDto?> GetPurchase(int id);
    Task<PaginatedPurchase> GetPurchases(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null);
}