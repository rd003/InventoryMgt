using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
using InventoryMgt.Data.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InventoryMgt.Data.Repositories;

public interface IProductRepository
{
    Task<ProductDisplay> AddProduct(Product product);
    Task<ProductDisplay> UpdatProduct(Product product);
    Task DeleteProduct(int id);
    Task<PagedProduct> GetProducts(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? @sortDirection = null);
    Task<ProductDisplay?> GetProduct(int id);
    Task<IEnumerable<ProductWithStock>> GetAllProductsWithStock();
}
public class ProductRepository : IProductRepository
{
    private readonly IConfiguration _config;
    private readonly string? _constr;
    public ProductRepository(IConfiguration config)
    {
        _config = config;
        _constr = _config.GetConnectionString("default");
    }
    public async Task<ProductDisplay> AddProduct(Product product)
    {
        using IDbConnection connection = new NpgsqlConnection(_constr);
        string sql = @"insert into 
                    product (product_name,category_id,supplier_id,price)
                    values(@ProductName,@CategoryId,@SupplierId,@Price) 
                    returning id;";
        int createdProductId = await connection.ExecuteScalarAsync<int>(sql, product);
        ProductDisplay? createdProduct = await GetProduct(createdProductId);
        return createdProduct!; // I am sure, it won't be null
    }

    public async Task<ProductDisplay> UpdatProduct(Product product)
    {
        using IDbConnection connection = new NpgsqlConnection(_constr);
        string sql = @"update product 
                        set product_name=@ProductName,
                        category_id=@CategoryId,
                        supplier_id=@SupplierId,
                        price=@Price
                       where id=@Id;";
        await connection.ExecuteAsync(sql, product);
        var updatedProduct = await GetProduct(product.Id);
        return updatedProduct!;
    }

    public async Task DeleteProduct(int id)
    {
        using IDbConnection connection = new NpgsqlConnection(_constr);
        string sql = "update product set is_deleted=true where id=@id";
        await connection.ExecuteAsync(sql, new { id });
    }

    public async Task<ProductDisplay?> GetProduct(int id)
    {
        using IDbConnection connection = new NpgsqlConnection(_constr);
        string sql = @"select 
                        p.id,
                        p.product_name,
                        p.price,
                        s.supplier_name,
                        c.category_name 
                    from product p 
                    join category c on p.category_id = c.id
                    join supplier s on p.supplier_id = s.id
                    where p.is_deleted=false and c.is_deleted=false and p.id = @Id";
        var product = await connection.QueryFirstOrDefaultAsync<ProductDisplay>(sql, new { id });
        return product;
    }

    public async Task<PagedProduct> GetProducts(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? @sortDirection = null)
    {
        using IDbConnection connection = new NpgsqlConnection(_constr);
        var result = await connection.QueryMultipleAsync("usp_getProducts", new
        {
            page,
            limit,
            searchTerm,
            sortColumn,
            sortDirection
        }, commandType: CommandType.StoredProcedure);
        var products = await result.ReadAsync<ProductDisplay>();
        var productCountResult = await result.ReadFirstAsync<ProductCount>();
        return new PagedProduct
        {
            Products = products,
            TotalPages = productCountResult.TotalPages,
            TotalRecords = productCountResult.TotalRecords,
            Page = page,
            Limit = limit,
        };
    }

    public async Task<IEnumerable<ProductWithStock>> GetAllProductsWithStock()
    {
        using IDbConnection connection = new NpgsqlConnection(_constr);
        var products = await connection.QueryAsync<ProductWithStock>("usp_GetAllProductsWithStock", commandType: CommandType.StoredProcedure);
        return products;
    }
}