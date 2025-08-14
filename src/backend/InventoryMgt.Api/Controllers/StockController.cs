using System.Text.Json;
using InventoryMgt.Api.CustomExceptions;
using InventoryMgt.Data.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMgt.Api.Controllers;
[ApiController]
[Route("/api/stocks")]
public class StockController : ControllerBase
{
    private readonly IStockRepository _stockRepo;
    public StockController(IStockRepository stockRepo)
    {
        _stockRepo = stockRepo;
    }
    [HttpGet]
    public async Task<IActionResult> GetStocks(int page = 1, int limit = 4, string sortColumn = "Id", string sortDirection = "asc", string? searchTerm = null)
    {
        if (sortDirection != null && !new[] { "asc", "desc" }.Contains(sortDirection))
        {
            throw new BadRequestException("'sortDirection' accepts values 'asc' and 'desc' only");
        }

        var allowedSortColumns = new[] { "Id", "ProductName", "CategoryName" };
        if (sortColumn != null && !allowedSortColumns.Contains(sortColumn))
        {
            throw new BadRequestException($"only {string.Join(',', allowedSortColumns)} columns allowed");
        }
        PaginatedStock paginatedStock = await _stockRepo.GetStocks(page, limit, sortColumn, sortDirection, searchTerm);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginatedStock.Pagination));
        return Ok(paginatedStock.Stocks);
    }
}