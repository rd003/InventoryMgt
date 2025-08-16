using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
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

    public async Task<SaleReadDto> AddSale(Sale sale)
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
    public async Task<SaleReadDto> UpdateSale(Sale sale)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        var updatedSale = await connection.QueryFirstAsync<SaleReadDto>("usp_UpdateSale", new
        {
            sale.Id,
            sale.SellingDate,
            sale.ProductId,
            sale.Description,
            sale.Quantity,
            sale.Price
        }, commandType: CommandType.StoredProcedure);
        return updatedSale;
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
        var multi = await connection.QueryMultipleAsync("usp_GetSales", new
        {
            page,
            limit,
            productName,
            dateFrom,
            dateTo,
            sortColumn,
            sortDirection
        }, commandType: CommandType.StoredProcedure);
        var sales = multi.Read<SaleReadDto>();
        var paginationData = multi.ReadFirst<PaginationBase>();
        var paginatedSale = new PaginatedSale
        {
            Sales = sales,
            Pagination = paginationData
        };
        return paginatedSale;
    }

    public async Task RemoveSale(int id)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync("usp_DeleteSale", new { id });
    }
}
