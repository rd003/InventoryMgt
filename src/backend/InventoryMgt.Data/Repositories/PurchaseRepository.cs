using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;

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
    public async Task<Purchase> AddPurchase(Purchase purchase)
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

    public async Task<Purchase> UpdatePurchase(Purchase purchase)
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

    public async Task<Purchase?> GetPurchase(int purchaseId)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        var purchase = await connection.QuerySingleOrDefaultAsync<Purchase>(@"
            SELECT p.id, 
                p.create_date, 
                p.update_date, 
                p.is_deleted,
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
        var param = new
        {
            page,
            limit,
            productName,
            dateFrom,
            dateTo,
            sortColumn,
            sortDirection
        };
        var multipleResult = await connection.QueryMultipleAsync("usp_getPurchases", param, commandType: CommandType.StoredProcedure);
        var purchases = multipleResult.Read<PurchaseReadDto>();
        var paginationData = multipleResult.ReadFirst<PaginationBase>();
        paginationData.Page = page;
        paginationData.Limit = limit;
        return new PaginatedPurchase { Purchases = purchases, Pagination = paginationData };
    }

    public async Task RemovePurchase(int id)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync("usp_DeletePurchase", new
        {
            Id = id
        }, commandType: CommandType.StoredProcedure);
    }


}