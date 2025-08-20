using System.Data;
using Dapper;
using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InventoryMgt.Shared.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly string? _connectionString;
    private readonly IConfiguration _config;
    public CategoryRepository(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("default");
    }
    public async Task<CategoryReadDto> AddCategory(CategoryDto category)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);

        // Single query approach
        // string sql = @"insert into category(category_name, category_id)
        //             values (@CategoryName, @CategoryId);

        //             select 
        //                 c.id,
        //                 c.category_id,
        //                 c.category_name,
        //                 parent.category_name as parent_category_name
        //             from category c
        //             left join category parent on c.category_id = parent.id
        //             where c.id = lastval();";

        // I am chosing the round trip, because I am using the same select query in 3 places. Later, I found it a maintainence over head so I am chosing roundtrip approach.

        string sql = @"insert into category(category_name, category_id)
                    values (@CategoryName, @CategoryId) 
                    returning id";
        var createdCategoryId = await connection.ExecuteScalarAsync<int>(sql, new
        {
            category.CategoryName,
            category.CategoryId
        });

        var createdCategory = await GetCategory(createdCategoryId);
        return createdCategory!; // I am positive, it won't return null
    }

    public async Task DeleteCategory(int id)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        string sql = "update Category set is_deleted=@IsDeleted where id=@Id";
        await connection.ExecuteAsync(sql, new
        {
            IsDeleted = true,
            Id = id
        });
    }

    public async Task<IEnumerable<CategoryReadDto>> GetCategories(string searchTerm = "")
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);

        string sql = @"
                    SELECT 
                        c.id,
                        c.category_id,
                        c.category_name,
                        parent.category_name AS parent_category_name
                    FROM category c
                    LEFT JOIN category parent
                        ON c.category_id = parent.id
                    WHERE c.is_deleted = false";
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            sql += " AND c.category_name ILIKE @SearchTerm || '%'";
        }
        sql += " order by c.category_name";
        return await connection.QueryAsync<CategoryReadDto>(sql, new { searchTerm });

    }

    public async Task<CategoryReadDto?> GetCategory(int id)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        string sql = @"select 
                        c.id,
                        c.category_id,
                        c.category_name,
                        parent.category_name as parent_category_name
                    from category c
                    left join category parent on c.category_id = parent.id
                    where c.is_deleted=false and c.id=@Id";
        return await connection.QueryFirstOrDefaultAsync<CategoryReadDto>(sql, new { Id = id });
    }

    public async Task<CategoryReadDto> UpdateCategory(CategoryDto category)
    {
        // category.UpdateDate = DateTime.UtcNow;
        using IDbConnection connection = new NpgsqlConnection(_connectionString);

        string sql = @"update category
          set category_name=@CategoryName,
          category_id=@CategoryId 
          where id=@Id";
        await connection.ExecuteAsync(sql, category);
        var updatedCategory = await GetCategory(category.Id);
        return updatedCategory!; // I am positive, that it won't be null
    }
}