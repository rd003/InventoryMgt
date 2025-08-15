using InventoryMgt.Data.Models;

namespace InventoryMgt.Data.Repositories;

public interface IPurchaseRepository
{
    Task<Purchase> AddPurchase(Purchase purchase);
    Task<Purchase> UpdatePurchase(Purchase purchase);
    Task RemovePurchase(int id);
    Task<Purchase?> GetPurchase(int id);
    Task<PaginatedPurchase> GetPurchases(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null);
}