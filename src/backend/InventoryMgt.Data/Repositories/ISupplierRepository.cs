using InventoryMgt.Data.models.DTOs;
using InventoryMgt.Data.Models.DTOs;

namespace InventoryMgt.Data.Repositories;

public interface ISupplierRepository
{
    Task<SupplierReadDto> AddSupplierAsync(SupplierDto supplier);
    Task UpdateSupplierAsync(SupplierDto supplier);
    Task DeleteSupplierAsnc(int supplierId);
    Task<SupplierReadDto?> GetSupplierByIdAsnc(int supplierId);
    Task<PagedSupplier> GetSuppliersAsnc(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null);
}