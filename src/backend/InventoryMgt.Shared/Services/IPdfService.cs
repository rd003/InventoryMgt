using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Shared.Services;

public interface IPdfService
{
    /// <summary>
    /// Generates a PDF report for purchase records
    /// </summary>
    /// <param name="purchases">List of purchase records to include in the report</param>
    /// <param name="productName">Filter: Product name (if applied)</param>
    /// <param name="dateFrom">Filter: Start date (if applied)</param>
    /// <param name="dateTo">Filter: End date (if applied)</param>
    /// <param name="sortColumn">Sort column (if applied)</param>
    /// <param name="sortDirection">Sort direction (if applied)</param>
    /// <returns>PDF document as byte array</returns>
    byte[] GeneratePurchaseReportPdf(
        IEnumerable<PurchaseReadDto> purchases,
        string? productName = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        string? sortColumn = null,
        string? sortDirection = null);

    /// <summary>
    /// Generates a PDF report for sale records
    /// </summary>
    /// <param name="sales">List of sale records to include in the report</param>
    /// <param name="productName">Filter: Product name (if applied)</param>
    /// <param name="dateFrom">Filter: Start date (if applied)</param>
    /// <param name="dateTo">Filter: End date (if applied)</param>
    /// <param name="sortColumn">Sort column (if applied)</param>
    /// <param name="sortDirection">Sort direction (if applied)</param>
    /// <returns>PDF document as byte array</returns>
    byte[] GenerateSaleReportPdf(
        IEnumerable<SaleReadDto> sales,
        string? productName = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        string? sortColumn = null,
        string? sortDirection = null);
}

