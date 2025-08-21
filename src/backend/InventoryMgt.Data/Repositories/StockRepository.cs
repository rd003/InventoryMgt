using System.Data;
using Dapper;
using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InventoryMgt.Shared.Repositories;

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
                        p.sku,
                        c.category_name
                        from stock s
                        join product p on s.product_id = p.id 
                        join category c on p.category_id = c.id
                        where s.is_deleted=false  and s.product_id=@ProductId";

        var stock = await connection.QueryFirstOrDefaultAsync<StockDisplayModel>(sql, new { productId });
        return stock;
    }

    public async Task<PaginatedStock> GetStocks(int page = 1, int limit = 4, string sortColumn = "id", string sortDirection = "asc", string? searchTerm = null)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);

        // Set default sort parameters and normalize case
        sortColumn = string.IsNullOrWhiteSpace(sortColumn) ? "id" : sortColumn.ToLower();
        sortDirection = string.IsNullOrWhiteSpace(sortDirection) ? "asc" : sortDirection.ToLower();

        // Build the WHERE clause conditions
        var whereConditions = new List<string>();
        var parameters = new DynamicParameters();

        // Base conditions for soft delete
        whereConditions.Add("s.is_deleted = false AND p.is_deleted = false AND c.is_deleted = false");

        // Add search term filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            whereConditions.Add("(p.product_name ILIKE @searchTerm OR c.category_name ILIKE @searchTerm)");
            parameters.Add("@searchTerm", $"%{searchTerm}%");
        }

        // Build ORDER BY clause with validation
        var validSortColumns = new Dictionary<string, string>
    {
        { "id", "s.id" },
        { "productname", "p.product_name" },
        { "categoryname", "c.category_name" },
        { "quantity", "s.quantity" }
    };

        var orderByColumn = validSortColumns.ContainsKey(sortColumn) ? validSortColumns[sortColumn] : "s.id";
        var orderByDirection = sortDirection == "desc" ? "DESC" : "ASC";

        // Add pagination parameters
        parameters.Add("@limit", limit);
        parameters.Add("@offset", (page - 1) * limit);

        var whereClause = string.Join(" AND ", whereConditions);

        // Main query for stocks with joins
        var stocksQuery = $@"
        SELECT 
            s.id,
            s.product_id,
            s.quantity,
            p.product_name,
            p.sku,
            c.category_name
        FROM stock s
        INNER JOIN product p ON s.product_id = p.id
        INNER JOIN category c ON p.category_id = c.id
        WHERE {whereClause}
        ORDER BY {orderByColumn} {orderByDirection}
        LIMIT @limit OFFSET @offset";

        // Count query for pagination
        var countQuery = $@"
        SELECT 
            COUNT(s.id) as total_records,
            CAST(CEILING((COUNT(s.id)::decimal) / @limit) AS INTEGER) as total_pages
        FROM stock s
        INNER JOIN product p ON s.product_id = p.id
        INNER JOIN category c ON p.category_id = c.id
        WHERE {whereClause}";

        // Execute both queries
        var stocks = await connection.QueryAsync<StockDisplayModel>(stocksQuery, parameters);
        var paginationData = await connection.QueryFirstAsync<PaginationBase>(countQuery, parameters);

        paginationData.Page = page;
        paginationData.Limit = limit;

        return new PaginatedStock
        {
            Stocks = stocks,
            Pagination = paginationData
        };
    }
}