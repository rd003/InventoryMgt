using System.Text.Json;
using InventoryMgt.Api.CustomExceptions;
using InventoryMgt.Data.Models;
using InventoryMgt.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMgt.Api.Controllers;
[ApiController]
[Route("api/sales")]
public class SaleController(ISaleRepository saleRepository, IStockRepository stockRepository) : ControllerBase
{
    private readonly ISaleRepository _saleRepo = saleRepository;
    private readonly IStockRepository _stockRepo = stockRepository;

    [HttpGet]
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
        var response = await _saleRepo.GetSales(page, limit, productName, dateFrom, dateTo, sortColumn, sortDirection);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(response.Pagination));
        return Ok(response.Sales);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetSaleById(int id)
    {
        var sale = await _saleRepo.GetSale(id);
        if (sale == null)
            throw new NotFoundException($"Record with id: {id} does not found");
        return Ok(sale);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale(Sale sale)
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
    public async Task<IActionResult> UpdateSale(int id, [FromBody] Sale sale)
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