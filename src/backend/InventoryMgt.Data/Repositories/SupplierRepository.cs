using Dapper;
using InventoryMgt.Data.Models;
using InventoryMgt.Data.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InventoryMgt.Data.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly IConfiguration _config;
    private readonly string _constr;

    public SupplierRepository(IConfiguration config)
    {
        _config = config;
        _constr = _config.GetConnectionString("default") ?? throw new InvalidOperationException("Config 'default' not found");
    }

    public async Task<SupplierReadDto> AddSupplierAsync(Supplier supplier)
    {
        using var connection = new NpgsqlConnection(_constr);
        string sql = @"
        INSERT INTO supplier 
            (supplier_name, contact_person, email, phone, address, city, state, country, postal_code, tax_number, payment_terms)
        VALUES 
            (@SupplierName, @ContactPerson, @Email, @Phone, @Address, @City, @State, @Country, @PostalCode, @TaxNumber, @PaymentTerms)
        RETURNING id;
    ";

        int supplierId = await connection.ExecuteScalarAsync<int>(sql, supplier);

        return new SupplierReadDto
        {
            Id = supplierId,
            Address = supplier.Address,
            City = supplier.City,
            ContactPerson = supplier.ContactPerson,
            Country = supplier.Country,
            Email = supplier.Email,
            PaymentTerms = supplier.PaymentTerms,
            Phone = supplier.Phone,
            PostalCode = supplier.PostalCode,
            State = supplier.State,
            SupplierName = supplier.SupplierName,
            TaxNumber = supplier.TaxNumber
        };
    }

    public async Task UpdateSupplierAsync(Supplier supplier)
    {
        using var connection = new NpgsqlConnection(_constr);
        const string sql = @"
        UPDATE supplier SET
            supplier_name = @SupplierName,
            contact_person = @ContactPerson,
            email = @Email,
            phone = @Phone,
            address = @Address,
            city = @City,
            state = @State,
            country = @Country,
            postal_code = @PostalCode,
            tax_number = @TaxNumber,
            payment_terms = @PaymentTerms
        WHERE id = @Id;
    ";

        await connection.ExecuteAsync(sql, supplier);
    }

    public async Task DeleteSupplierAsnc(int supplierId)
    {
        using var connection = new NpgsqlConnection(_constr);
        const string sql = @"UPDATE supplier SET is_deleted = true
        WHERE id = @supplierId;";

        await connection.ExecuteAsync(sql, new { supplierId });
    }

    public async Task<SupplierReadDto?> GetSupplierByIdAsnc(int supplierId)
    {
        using var connection = new NpgsqlConnection(_constr);
        string sql = @"
        SELECT 
            id,
            supplier_name,
            contact_person,
            email,
            phone,
            address,
            city,
            state,
            country,
            postal_code,
            tax_number,
            payment_terms,
            is_active
        FROM supplier
        WHERE id = @supplierId and is_deleted = false
        ;
        ";
        var supplier = await connection.QueryFirstOrDefaultAsync<SupplierReadDto>(sql, new { supplierId });
        return supplier;
    }

    public async Task<PagedSupplier> GetSuppliersAsnc(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null)
    {
        // Validate parameters
        page = Math.Max(1, page);
        limit = Math.Max(1, Math.Min(100, limit)); // Cap limit at 100
        sortDirection = string.IsNullOrEmpty(sortDirection) ? "ASC" : sortDirection.ToUpper();

        // Validate sort direction
        if (sortDirection != "ASC" && sortDirection != "DESC")
            sortDirection = "ASC";

        // Validate and set default sort column
        var validColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "supplier_name", "contact_person"
    };

        if (string.IsNullOrEmpty(sortColumn) || !validColumns.Contains(sortColumn))
            sortColumn = "supplier_name";

        // Build the WHERE clause for search
        var whereClause = "WHERE is_deleted = false";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            whereClause += @" AND (
            supplier_name ILIKE @SearchTerm OR 
            contact_person ILIKE @SearchTerm
        )";
            parameters.Add("SearchTerm", $"{searchTerm}%");
        }

        // Calculate offset
        var offset = (page - 1) * limit;
        parameters.Add("Limit", limit);
        parameters.Add("Offset", offset);

        // Build the main query
        var mainQuery = $@"
        SELECT 
            id as Id,
            supplier_name,
            contact_person,
            email,
            phone,
            address,
            city,
            state,
            country,
            postal_code,
            tax_number,
            payment_terms,
            is_active
        FROM supplier
        {whereClause}
        ORDER BY {sortColumn} {sortDirection}
        LIMIT @Limit OFFSET @Offset";

        // Build the count query
        var countQuery = $@"
        SELECT COUNT(*) 
        FROM supplier 
        {whereClause}";

        using var connection = new NpgsqlConnection(_constr);

        // Execute both queries concurrently
        var suppliersTask = connection.QueryAsync<SupplierReadDto>(mainQuery, parameters);
        var countTask = connection.QuerySingleAsync<int>(countQuery, parameters);

        await Task.WhenAll(suppliersTask, countTask);

        var suppliers = await suppliersTask;
        var totalRecords = await countTask;

        // Calculate total pages
        var totalPages = (int)Math.Ceiling((double)totalRecords / limit);

        return new PagedSupplier
        {
            Suppliers = suppliers,
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            Page = page,
            Limit = limit
        };
    }

}