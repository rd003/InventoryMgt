using System.Data;
using Dapper;
using InventoryMgt.Data.models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InventoryMgt.Data.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public SaleRepository(IConfiguration configuration)
    {
        _config = configuration;
        _connectionString = _config.GetConnectionString("default");
    }

    public async Task<SaleReadDto> AddSale(SaleDto sale)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();
        int createdSaleId;
        try
        {
            // Insert sale and get the created ID
            var insertSaleQuery = @"
            INSERT INTO sale (product_id, selling_date, quantity, price, description)
            VALUES (@ProductId, @SellingDate, @Quantity, @Price, @Description)
            RETURNING id";

            createdSaleId = await connection.QuerySingleAsync<int>(insertSaleQuery, sale, transaction);

            // Note I already have validated stock in controller.

            var updateStockQuery = @"
            UPDATE stock 
            SET quantity = quantity - @Quantity 
            WHERE product_id = @ProductId";

            await connection.ExecuteAsync(updateStockQuery, new
            {
                sale.Quantity,
                sale.ProductId
            }, transaction);

            await transaction.CommitAsync();

        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        var createdSale = await GetSale(createdSaleId);

        return createdSale!;
    }
    public async Task<SaleReadDto> UpdateSale(SaleDto sale)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();

        try
        {
            // retrieving product id and it's stock before updating the sale entry
            var (previousProductId, previousQuantity) = await connection.QueryFirstAsync<(int, decimal)>(@"select product_id, quantity from sale where id=@Id", new { sale.Id }, transaction);

            // update sale entry
            string updateSql = @"update sale set update_date=now(),
                        product_id=@ProductId,
                        selling_date=@SellingDate,
                        quantity=@Quantity,
                        price=@Price,
                        description=@Description
	                    where id=@Id";
            await connection.ExecuteAsync(updateSql, sale, transaction);

            // manage stock
            // Manage stock based on product change
            if (previousProductId == sale.ProductId)
            {
                // Same product - adjust quantity difference
                var adjustStockQuery = @"
                UPDATE stock 
                SET quantity = quantity + @PreviousQuantity - @NewQuantity 
                WHERE product_id = @ProductId";

                await connection.ExecuteAsync(adjustStockQuery, new
                {
                    PreviousQuantity = previousQuantity,
                    NewQuantity = sale.Quantity,
                    sale.ProductId
                }, transaction);
            }
            else
            {
                // Different product - restore previous product stock and reduce new product stock

                // Increase quantity of previous product (return stock)
                var restorePreviousStockQuery = @"
                UPDATE stock 
                SET quantity = quantity + @PreviousQuantity 
                WHERE product_id = @PreviousProductId";

                await connection.ExecuteAsync(restorePreviousStockQuery, new
                {
                    PreviousQuantity = previousQuantity,
                    PreviousProductId = previousProductId
                }, transaction);

                // Check if stock exists for new product and validate quantity
                var newProductStock = await connection.ExecuteScalarAsync<decimal?>(
                    @"SELECT quantity FROM stock WHERE product_id = @ProductId",
                    new { sale.ProductId }, transaction);

                if (!newProductStock.HasValue)
                {
                    throw new InvalidOperationException("Stock is not available for the new product.");
                }

                if (newProductStock < sale.Quantity)
                {
                    throw new InvalidOperationException($"Not enough items in stock for new product ({newProductStock} available, {sale.Quantity} requested).");
                }

                // Decrease quantity of new product
                var reduceNewStockQuery = @"
                UPDATE stock 
                SET quantity = quantity - @Quantity 
                WHERE product_id = @ProductId";

                await connection.ExecuteAsync(reduceNewStockQuery, new
                {
                    sale.Quantity,
                    sale.ProductId
                }, transaction);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        // returning updated sale record
        return (await GetSale(sale.Id))!;
    }

    public async Task RemoveSale(int id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // Get product id and quantity before soft deleting the sale
            var (productId, quantity) = await connection.QueryFirstAsync<(int, decimal)>(
                @"SELECT product_id, quantity FROM sale WHERE id = @Id",
                new { Id = id }, transaction);

            // Soft delete the sale 
            await connection.ExecuteAsync(@"
            UPDATE sale 
            SET is_deleted = true 
            WHERE id = @Id", new { Id = id }, transaction);

            // Check if stock exists for this product
            var stockExists = await connection.ExecuteScalarAsync<bool>(
                @"SELECT EXISTS(SELECT 1 FROM stock WHERE product_id = @ProductId)",
                new { ProductId = productId }, transaction);

            if (stockExists)
            {
                // Restore the stock quantity (add back the sold quantity)
                await connection.ExecuteAsync(@"
                UPDATE stock 
                SET quantity = quantity + @Quantity 
                WHERE product_id = @ProductId", new
                {
                    Quantity = quantity,
                    ProductId = productId
                }, transaction);
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<SaleReadDto?> GetSale(int id)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        string sql = @"
                    select 
                    s.id,
                    s.selling_date,
                    s.description,
                    s.price,
                    s.quantity,
                    p.product_name,
                    p.sku 
                    from sale s 
                    join product p  on s.product_id=p.id
                    where s.is_deleted=false and p.is_deleted=false and s.id=@id
        ";
        var sale = await connection.QueryFirstOrDefaultAsync<SaleReadDto>(sql, new { id });
        return sale;
    }

    public async Task<PaginatedSale> GetSales(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);

        // Set default sort parameters
        sortColumn = string.IsNullOrWhiteSpace(sortColumn) ? "id" : sortColumn.ToLower();
        sortDirection = string.IsNullOrWhiteSpace(sortDirection) ? "asc" : sortDirection.ToLower();

        // Build the WHERE clause conditions
        var whereConditions = new List<string>();
        var parameters = new DynamicParameters();

        // Base conditions
        whereConditions.Add("s.is_deleted = false AND p.is_deleted = false");

        // Add date filters if provided
        if (dateFrom.HasValue && dateTo.HasValue)
        {
            whereConditions.Add("s.selling_date >= @dateFrom AND s.selling_date <= @dateTo");
            parameters.Add("@dateFrom", dateFrom.Value);
            parameters.Add("@dateTo", dateTo.Value);
        }

        // Add product name filter if provided
        if (!string.IsNullOrWhiteSpace(productName))
        {
            whereConditions.Add("p.product_name ILIKE @productName");
            parameters.Add("@productName", $"%{productName}%");
        }

        // Build ORDER BY clause
        var validSortColumns = new Dictionary<string, string>
    {
        { "id", "s.id" },
        { "productname", "p.product_name" },
        { "price", "s.price" },
        { "sellingdate", "s.selling_date" },
        { "quantity", "s.quantity" }
    };

        var orderByColumn = validSortColumns.ContainsKey(sortColumn) ? validSortColumns[sortColumn] : "s.id";
        var orderByDirection = sortDirection == "desc" ? "DESC" : "ASC";

        // Add pagination parameters
        parameters.Add("@limit", limit);
        parameters.Add("@offset", (page - 1) * limit);

        var whereClause = string.Join(" AND ", whereConditions);

        // Main query for sales
        var salesQuery = $@"
        SELECT 
            s.id,
            s.product_id,
            s.selling_date,
            s.quantity,
            s.price,
            s.description,
            p.product_name,
            p.sku
        FROM sale s
        INNER JOIN product p ON s.product_id = p.id
        WHERE {whereClause}
        ORDER BY {orderByColumn} {orderByDirection}
        LIMIT @limit OFFSET @offset";

        // Count query for pagination
        var countQuery = $@"
        SELECT 
            COUNT(s.id) as total_records,
            CAST(CEILING((COUNT(s.id)::decimal) / @limit) AS INTEGER) as total_pages
        FROM sale s
        INNER JOIN product p ON s.product_id = p.id
        WHERE {whereClause}";

        // Execute both queries
        var sales = await connection.QueryAsync<SaleReadDto>(salesQuery, parameters);
        var paginationData = await connection.QueryFirstAsync<PaginationBase>(countQuery, parameters);

        paginationData.Page = page;
        paginationData.Limit = limit;

        return new PaginatedSale
        {
            Sales = sales,
            Pagination = paginationData
        };
    }


}
