using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InventoryMgt.Data.Repositories;


public interface ISaleRepository
{
    Task<Sale> AddSale(Sale sale);
    Task<Sale> UpdateSale(Sale sale);
    Task RemoveSale(int id);
    Task<Sale?> GetSale(int id);
    Task<PaginatedSale> GetSales(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null);

}
public class SaleRepository : ISaleRepository
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public SaleRepository(IConfiguration configuration)
    {
        _config = configuration;
        _connectionString = _config.GetConnectionString("default");
    }

    public async Task<Sale> AddSale(Sale sale)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        var createdSale = await connection.QueryFirstAsync<Sale>("usp_AddSale", new
        {
            sale.SellingDate,
            sale.ProductId,
            sale.Description,
            sale.Quantity,
            sale.Price
        }, commandType: CommandType.StoredProcedure);
        return createdSale;
    }

    public async Task<Sale?> GetSale(int id)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        var sale = await connection.QueryFirstOrDefaultAsync<Sale>("usp_GetSaleById", new { id });
        return sale;
    }

    public async Task<PaginatedSale> GetSales(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
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
        var sales = multi.Read<Sale>();
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
        using IDbConnection connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("usp_DeleteSale", new { id });
    }

    public async Task<Sale> UpdateSale(Sale sale)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        var updatedSale = await connection.QueryFirstAsync<Sale>("usp_UpdateSale", new
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
}
