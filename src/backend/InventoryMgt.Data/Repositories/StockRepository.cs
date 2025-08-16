using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
using InventoryMgt.Data.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InventoryMgt.Data.Repositories;

public class StockRepository : IStockRepository
{
    private readonly IConfiguration _config;
    private readonly string? _connectionString;
    public StockRepository(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("default");
    }

    public async Task<StockDisplayModel?> GetStockByProductId(int productId)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        string sql = @"select 
                        s.id,
                        s.product_id,
                        s.quantity,
                        p.product_name,
                        c.category_name
                        from stock s
                        join product p on s.product_id = p.id 
                        join category c on p.category_id = c.id
                        where s.is_deleted=false  and s.product_id=@ProductId";

        var stock = await connection.QueryFirstOrDefaultAsync<StockDisplayModel>(sql, new { productId });
        return stock;
    }

    public async Task<PaginatedStock> GetStocks(int page = 1, int limit = 4, string sortColumn = "Id", string sortDirection = "asc", string? searchTerm = null)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        var multiResult = await connection.QueryMultipleAsync("usp_GetStock", new { page, limit, sortColumn, sortDirection, searchTerm }, commandType: CommandType.StoredProcedure);
        IEnumerable<StockDisplayModel> stocks = multiResult.Read<StockDisplayModel>();
        PaginationBase paginatedData = multiResult.ReadFirst<PaginationBase>();
        paginatedData.Page = page;
        paginatedData.Limit = limit;
        return new PaginatedStock { Stocks = stocks, Pagination = paginatedData };
    }
}