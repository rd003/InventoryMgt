using System.Data;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using InventoryMgt.Data.models.DTOs;

namespace InventoryMgt.Data.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    public PurchaseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("default") ?? "";
    }
    public async Task<PurchaseDto> AddPurchase(PurchaseDto purchase)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // NOTE: I have managed it with single round trip using stored procedure in sql server. Since I m migrating, so I m just trying to get work done.
            // May be I will refactor it in future if needed.

            // Insert purchase and get ID
            var purchaseId = await connection.ExecuteScalarAsync<int>(@"
            INSERT INTO purchase (product_id, purchase_date, quantity, unit_price, description,purchase_order_number,invoice_number,received_date)
            VALUES (@ProductId, @PurchaseDate, @Quantity, @UnitPrice, @Description,@PurchaseOrderNumber,@InvoiceNumber,@ReceivedDate)
            RETURNING id",
              purchase, transaction);

            // Upsert stock
            await connection.ExecuteAsync(@"
            INSERT INTO stock (product_id, quantity)
            VALUES (@productId, @quantity)
            ON CONFLICT (product_id)
            DO UPDATE SET quantity = stock.quantity + @quantity",
                new { purchase.ProductId, purchase.Quantity }, transaction);

            await transaction.CommitAsync();

            // Return created purchase with product info
            var result = await GetPurchase(purchaseId);

            return result!;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PurchaseDto> UpdatePurchase(PurchaseDto purchase)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();

        try
        {
            var (previousProductId, previousQuantity) = await connection.QueryFirstAsync<(int, decimal)>(@"select product_id, quantity from Purchase where id=@Id", new { purchase.Id }, transaction);

            string updateSql = @"update  purchase 
                                set update_date=now(),
                                product_id=@ProductId,
                                purchase_date=@PurchaseDate,
                                quantity=@quantity,
                                unit_price=@UnitPrice,
                                description=@Description
                                where id=@Id";
            await connection.ExecuteAsync(updateSql, purchase, transaction);

            // Update stock

            if (previousProductId == purchase.ProductId)
            {
                // Same product: adjust the difference in quantity
                await connection.ExecuteAsync(@"
                update stock 
                set quantity = quantity - @PreviousQuantity + @NewQuantity 
                where product_id = @ProductId",
                    new
                    {
                        PreviousQuantity = previousQuantity,
                        NewQuantity = purchase.Quantity,
                        purchase.ProductId
                    }, transaction);
            }
            // if product is different
            else
            {
                // decrease the stock of prev product
                // increase the stock of new product if exists otherwise create new entry

                // Different product: decrease previous product quantity
                await connection.ExecuteAsync(@"
                update stock 
                set quantity = quantity - @PreviousQuantity 
                where product_id = @PreviousProductId",
                    new
                    {
                        PreviousQuantity = previousQuantity,
                        PreviousProductId = previousProductId
                    }, transaction);

                // Check if new product exists in stock
                var stockExists = await connection.ExecuteScalarAsync<int?>(@"
                select 1 from stock where product_id = @ProductId",
                    new { purchase.ProductId }, transaction);

                if (stockExists.HasValue)
                {
                    // Update existing stock
                    await connection.ExecuteAsync(@"
                    update stock 
                    set quantity = quantity + @Quantity 
                    where product_id = @ProductId",
                        new
                        {
                            purchase.Quantity,
                            purchase.ProductId
                        }, transaction);
                }
                else
                {
                    // Insert new stock record
                    await connection.ExecuteAsync(@"
                    insert into stock (product_id, quantity) 
                    values (@ProductId, @Quantity)",
                        new
                        {
                            purchase.ProductId,
                            purchase.Quantity
                        }, transaction);
                }
            }

            await transaction.CommitAsync();

            // return updated purchase entry
            var updatedPurchase = await GetPurchase(purchase.Id);
            return updatedPurchase!;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

    }

    public async Task RemovePurchase(int purchaseId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            // extracting productId and quantity of before deleting the entry
            var (existingProductId, existingQuantity) = await connection.QueryFirstAsync<(int, decimal)>(@"select product_id, quantity from purchase where id=@purchaseId", new { purchaseId }, transaction);

            // Check if we have enough stock to decrease
            var currentStock = await connection.ExecuteScalarAsync<decimal?>(
                @"select quantity from stock where product_id=@existingProductId",
                new { existingProductId }, transaction);

            if (!currentStock.HasValue)
            {
                throw new InvalidOperationException($"Stock record not found for product {existingProductId}");
            }
            if (currentStock.Value < existingQuantity)
            {
                throw new InvalidOperationException($"Insufficient stock. Current: {currentStock.Value}, Required: {existingQuantity}");
            }

            // Soft delete the purchase entry and decrease the stock
            await connection.ExecuteAsync(@"
                                update purchase set is_deleted=true, update_date=now() where id=@purchaseId;
                                
                                update stock set quantity=quantity-@existingQuantity where product_id=@existingProductId;
    ", new { purchaseId, existingProductId, existingQuantity }, transaction);

            await transaction.CommitAsync();

        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<PurchaseDto?> GetPurchase(int purchaseId)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        var purchase = await connection.QuerySingleOrDefaultAsync<PurchaseDto>(@"
            SELECT p.id,
                p.product_id, 
                p.purchase_date, 
                p.quantity, 
                p.unit_price, 
                p.description,
                p.purchase_order_number,
				p.invoice_number,
				p.received_date,
                pr.product_name,
                pr.sku
            FROM purchase p 
            INNER JOIN product pr ON p.product_id = pr.id
            WHERE p.id = @purchaseId 
              AND p.is_deleted = false 
              AND pr.is_deleted = false",
                new { purchaseId });
        return purchase;
    }

    public async Task<PaginatedPurchase> GetPurchases(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);

        // Set default sort parameters
        sortColumn = string.IsNullOrWhiteSpace(sortColumn) ? "id" : sortColumn.ToLower();
        sortDirection = string.IsNullOrWhiteSpace(sortDirection) ? "asc" : sortDirection.ToLower();

        // Build the WHERE clause conditions
        var whereConditions = new List<string>();
        var parameters = new DynamicParameters();

        // Base conditions
        whereConditions.Add("p.is_deleted = false AND pr.is_deleted = false");

        // Add date filters if provided
        if (dateFrom.HasValue && dateTo.HasValue)
        {
            whereConditions.Add("p.purchase_date >= @dateFrom AND p.purchase_date <= @dateTo");
            parameters.Add("@dateFrom", dateFrom.Value);
            parameters.Add("@dateTo", dateTo.Value);
        }

        // Add product name filter if provided
        if (!string.IsNullOrWhiteSpace(productName))
        {
            whereConditions.Add("pr.product_name ILIKE @productName");
            parameters.Add("@productName", $"%{productName}%");
        }

        // Build ORDER BY clause
        var validSortColumns = new Dictionary<string, string>
    {
        { "id", "p.id" },
        { "productname", "pr.product_name" },
        { "price", "p.price" },
        { "purchasedate", "p.purchase_date" }
    };

        var orderByColumn = validSortColumns.ContainsKey(sortColumn) ? validSortColumns[sortColumn] : "p.id";
        var orderByDirection = sortDirection == "desc" ? "DESC" : "ASC";

        // Add pagination parameters
        parameters.Add("@limit", limit);
        parameters.Add("@offset", (page - 1) * limit);

        var whereClause = string.Join(" AND ", whereConditions);

        // Main query for purchases
        var purchaseQuery = $@"SELECT 
                p.id,
                p.product_id, 
                p.purchase_date, 
                p.quantity, 
                p.unit_price, 
                p.description,
                p.purchase_order_number,
				p.invoice_number,
				p.received_date,
                pr.product_name,
                pr.sku
        FROM purchase p
        INNER JOIN product pr ON p.product_id = pr.id
        WHERE {whereClause}
        ORDER BY {orderByColumn} {orderByDirection}
        LIMIT @limit OFFSET @offset";

        // Count query for pagination
        var countQuery = $@"
        SELECT 
            COUNT(p.id) as total_records,
            CAST(CEILING((COUNT(p.id)::decimal) / @limit) AS INTEGER) as total_pages
        FROM purchase p
        INNER JOIN product pr ON p.product_id = pr.id
        WHERE {whereClause}";

        // Execute both queries
        var purchases = await connection.QueryAsync<PurchaseReadDto>(purchaseQuery, parameters);
        var paginationData = await connection.QueryFirstAsync<PaginationBase>(countQuery, parameters);

        paginationData.Page = page;
        paginationData.Limit = limit;

        return new PaginatedPurchase
        {
            Purchases = purchases,
            Pagination = paginationData
        };
    }

}