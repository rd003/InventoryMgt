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
            VALUES (@productId, @purchaseDate, @quantity, @price, @description,@PurchaseOrderNumber,@InvoiceNumber,@ReceivedDate)
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
            var result = await connection.QuerySingleAsync<Purchase>(@"
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
                pr.product_name
            FROM purchase p 
            INNER JOIN product pr ON p.product_id = pr.id
            WHERE p.id = @purchaseId 
              AND p.is_deleted = false 
              AND pr.is_deleted = false",
                new { purchaseId });

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Purchase?> GetPurchase(int id)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        var purchase = await connection.QueryFirstOrDefaultAsync<Purchase>("usp_GetPurchaseById", new { Id = id }, commandType: CommandType.StoredProcedure);
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

    public async Task<Purchase> UpdatePurchase(Purchase purchase)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        Purchase updatedPurchase = await connection.QuerySingleAsync<Purchase>("usp_UpdatePurchase", new
        {
            purchase.Id,
            purchase.PurchaseDate,
            purchase.ProductId,
            purchase.Description,
            purchase.Quantity,
            purchase.UnitPrice
        }, commandType: CommandType.StoredProcedure);
        return updatedPurchase;
    }
}