using System.Text.Json;
using InventoryMgt.Shared.CustomExceptions;
using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.Repositories;
using InventoryMgt.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMgt.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/sales")]
public class SaleController : ControllerBase
{
    private readonly ISaleRepository _saleRepo;
    private readonly IStockRepository _stockRepo;
    private readonly IPdfService _pdfService;

    public SaleController(ISaleRepository saleRepository, IStockRepository stockRepository, IPdfService pdfService)
    {
        _saleRepo = saleRepository;
        _stockRepo = stockRepository;
        _pdfService = pdfService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetSales(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null)
    {
        if (sortDirection != null && !new[] { "asc", "desc" }.Contains(sortDirection))
        {
            throw new BadRequestException("'sortDirection' accepts values 'asc' and 'desc' only");
        }

        var allowedSortColumns = new[] { "Id", "ProductName", "Price", "CreateDate", "UpdateDate", "SellingDate" };
        if (sortColumn != null && !allowedSortColumns.Contains(sortColumn))
        {
            throw new BadRequestException($"only {string.Join(",", allowedSortColumns)} are allowed as sortColumn");
        }

        // Check if PDF is requested via Accept header
        var acceptHeader = Request.Headers["Accept"].ToString();
        if (acceptHeader.Contains("application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            // For PDF, get all matching sales (no pagination)
            var allSales = await _saleRepo.GetSales(1, int.MaxValue, productName, dateFrom, dateTo, sortColumn, sortDirection);
            var pdfBytes = _pdfService.GenerateSaleReportPdf(
                allSales.Sales,
                productName,
                dateFrom,
                dateTo,
                sortColumn,
                sortDirection);

            var fileName = $"SalesReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        // Default: return JSON
        var response = await _saleRepo.GetSales(page, limit, productName, dateFrom, dateTo, sortColumn, sortDirection);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(response.Pagination));
        return Ok(response.Sales);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSaleById(int id)
    {
        var sale = await _saleRepo.GetSale(id);
        if (sale == null)
            throw new NotFoundException($"Record with id: {id} does not found");
        return Ok(sale);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale(SaleDto sale)
    {
        var stock = await _stockRepo.GetStockByProductId(sale.ProductId);
        if (stock == null)
            throw new BadRequestException("This product is not in stock");
        if (sale.Quantity > stock.Quantity)
            throw new BadRequestException($"You can not sell more than ${stock.Quantity} items");
        var createdSale = await _saleRepo.AddSale(sale);
        return CreatedAtAction(nameof(CreateSale), createdSale);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSale(int id, [FromBody] SaleDto sale)
    {
        if (sale.Id != id)
            throw new BadHttpRequestException("id in url and body does not match");
        var exitingSale = await _saleRepo.GetSale(id);
        if (exitingSale == null)
            throw new NotFoundException($"Record with id: {id} does not found");
        var updatedSale = await _saleRepo.UpdateSale(sale);
        return Ok(updatedSale);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(int id)
    {
        var exitingSale = await _saleRepo.GetSale(id);
        if (exitingSale == null)
            throw new NotFoundException($"Record with id: {id} does not found");
        await _saleRepo.RemoveSale(id);
        return NoContent();
    }
}