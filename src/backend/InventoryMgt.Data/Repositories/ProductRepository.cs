using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
using InventoryMgt.Data.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InventoryMgt.Data.Repositories;

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

    public async Task<PagedProduct> GetProducts(int page = 1, int limit = 4, string? searchTerm = "", string? sortColumn = "id", string? sortDirection = "asc")
    {
        var offset = (page - 1) * limit;
        var searchPattern = string.IsNullOrWhiteSpace(searchTerm) ? null : $"{searchTerm}%";

        var (productQuery, countQuery) = GetQueriesForSort(sortColumn?.ToLower(), sortDirection?.ToLower());

        var parameters = new
        {
            searchTerm = searchPattern,
            offset,
            limit
        };

        using var connection = new NpgsqlConnection(_constr);
        using var result = await connection.QueryMultipleAsync($"{productQuery}; {countQuery}", parameters);

        var products = await result.ReadAsync<ProductDisplay>();
        var productCountResult = await result.ReadFirstAsync<ProductCount>();

        return new PagedProduct
        {
            Products = products,
            TotalPages = productCountResult.TotalPages,
            TotalRecords = productCountResult.TotalRecords,
            Page = page,
            Limit = limit
        };
    }

    private static (string productQuery, string countQuery) GetQueriesForSort(string sortColumn, string sortDirection)
    {
        var baseProductQuery = @"
        SELECT 
        p.id, 
        p.product_name, 
        p.price, 
        p.category_id, 
        c.category_name,
        s.supplier_name
        FROM product p 
        INNER JOIN  category c ON p.category_id = c.id
        INNER JOIN  supplier s on p.supplier_id = s.id
        WHERE p.is_deleted = false 
        AND c.is_deleted = false
        AND s.is_deleted = false
        AND (@SearchTerm IS NULL 
        OR p.product_name ILIKE @SearchTerm 
        OR c.category_name ILIKE @SearchTerm
        OR s.supplier_name ILIKE @SearchTerm
        )";

        var baseCountQuery = @"
        SELECT COUNT(p.id) as total_records,
               CAST(CEILING(COUNT(p.id)::decimal / @limit) AS int) as total_pages
        FROM product p 
        INNER JOIN  category c ON p.category_id = c.id
        INNER JOIN  supplier s on p.supplier_id = s.id
        WHERE p.is_deleted = false 
          AND c.is_deleted = false
          AND s.is_deleted = false
          AND (@SearchTerm IS NULL 
        OR p.product_name ILIKE @SearchTerm 
        OR c.category_name ILIKE @SearchTerm
        OR s.supplier_name ILIKE @SearchTerm
        )";

        var orderBy = (sortColumn, sortDirection) switch
        {
            ("id", "desc") => "ORDER BY p.id DESC",
            ("product_name", "asc") => "ORDER BY p.product_name ASC",
            ("product_name", "desc") => "ORDER BY p.product_name DESC",
            ("price", "asc") => "ORDER BY p.price ASC",
            ("price", "desc") => "ORDER BY p.price DESC",
            ("create_date", "asc") => "ORDER BY p.create_date ASC",
            ("create_date", "desc") => "ORDER BY p.create_date DESC",
            ("update_date", "asc") => "ORDER BY p.update_date ASC",
            ("update_date", "desc") => "ORDER BY p.update_date DESC",
            ("category_name", "asc") => "ORDER BY c.category_name ASC",
            ("category_name", "desc") => "ORDER BY c.category_name DESC",
            ("supplier_name", "asc") => "ORDER BY c.supplier_name ASC",
            ("supplier_name", "desc") => "ORDER BY c.supplier_name DESC",
            _ => "ORDER BY p.id ASC" // default
        };

        var productQuery = $"{baseProductQuery} {orderBy} OFFSET @offset LIMIT @limit";

        return (productQuery, baseCountQuery);
    }

    public async Task<IEnumerable<ProductWithStock>> GetAllProductsWithStock()
    {
        using IDbConnection connection = new NpgsqlConnection(_constr);
        string sql = @"select 
                        p.id,
                        p.product_name, 
                        c.category_name,
                        p.price,
                    coalesce(s.Quantity,0) as quantity 
                    from product p
                    join category c on p.category_id=c.id
                    left join stock s on p.id = s.product_id
                    where p.is_deleted=false and c.is_deleted= false";

        var products = await connection.QueryAsync<ProductWithStock>(sql);
        return products;
    }
}