using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
using InventoryMgt.Data.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public interface IStockRepository
{
    Task<PaginatedStock> GetStocks(int page = 1, int limit = 4, string sortColumn = "Id", string sortDirection = "asc", string? searchTerm = null);
    Task<Stock?> GetStockByProductId(int productId);
}
public class StockRepository : IStockRepository
{
    private readonly IConfiguration _config;
    private readonly string? _connectionString;
    public StockRepository(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("default");
    }

    public async Task<Stock?> GetStockByProductId(int productId)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        var stock = await connection.QueryFirstOrDefaultAsync<StockDisplayModel>("usp_GetStockByProductId", new { productId }, commandType: CommandType.StoredProcedure);
        return stock;
    }

    public async Task<PaginatedStock> GetStocks(int page = 1, int limit = 4, string sortColumn = "Id", string sortDirection = "asc", string? searchTerm = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        var multiResult = await connection.QueryMultipleAsync("usp_GetStock", new { page, limit, sortColumn, sortDirection, searchTerm }, commandType: CommandType.StoredProcedure);
        IEnumerable<StockDisplayModel> stocks = multiResult.Read<StockDisplayModel>();
        PaginationBase paginatedData = multiResult.ReadFirst<PaginationBase>();
        paginatedData.Page = page;
        paginatedData.Limit = limit;
        return new PaginatedStock { Stocks = stocks, Pagination = paginatedData };
    }
}