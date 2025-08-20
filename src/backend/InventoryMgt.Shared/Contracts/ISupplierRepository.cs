using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Shared.Repositories;

public interface ISupplierRepository
{
    Task<SupplierReadDto> AddSupplierAsync(SupplierDto supplier);
    Task UpdateSupplierAsync(SupplierDto supplier);
    Task DeleteSupplierAsnc(int supplierId);
    Task<SupplierReadDto?> GetSupplierByIdAsnc(int supplierId);
    Task<PagedSupplier> GetSuppliersAsnc(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null);
}