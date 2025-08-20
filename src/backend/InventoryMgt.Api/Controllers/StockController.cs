using System.Text.Json;
using InventoryMgt.Shared.CustomExceptions;
using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.Repositories;
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
    [HttpGet]
    public async Task<IActionResult> GetStocks(int page = 1, int limit = 4, string sortColumn = "id", string sortDirection = "asc", string? searchTerm = null)
    {
        if (sortDirection != null && !new[] { "asc", "desc" }.Contains(sortDirection.ToLower()))
        {
            throw new BadRequestException("'sortDirection' accepts values 'asc' and 'desc' only");
        }

        var allowedSortColumns = new[] { "id", "productname", "categoryname", "quantity" };
        if (sortColumn != null && !allowedSortColumns.Contains(sortColumn.ToLower()))
        {
            throw new BadRequestException($"Only {string.Join(", ", allowedSortColumns)} columns allowed");
        }

        PaginatedStock paginatedStock = await _stockRepo.GetStocks(page, limit, sortColumn, sortDirection, searchTerm);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginatedStock.Pagination));
        return Ok(paginatedStock.Stocks);
    }
}