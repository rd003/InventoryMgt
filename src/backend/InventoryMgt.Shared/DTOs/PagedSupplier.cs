using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Shared.DTOs;

public class PagedSupplier : PaginationBase
{
    public IEnumerable<SupplierReadDto> Suppliers { get; set; } = [];
}

public class SupplierCount
{
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
}