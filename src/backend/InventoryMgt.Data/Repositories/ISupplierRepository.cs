using InventoryMgt.Data.Models;
using InventoryMgt.Data.Models.DTOs;

namespace InventoryMgt.Data.Repositories;

public interface ISupplierRepository
{
    Task<SupplierReadDto> AddSupplierAsync(Supplier supplier);
    Task UpdateSupplierAsync(Supplier supplier);
    Task DeleteSupplierAsnc(int supplierId);
    Task<SupplierReadDto?> GetSupplierByIdAsnc(int supplierId);
    Task<PagedSupplier> GetSuppliersAsnc(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null);
}