using System.Text.Json;
using InventoryMgt.Data.Constants;
using InventoryMgt.Data.Models;
using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryMgt.Data.Extensions;

public static class DatabaseInitializer
{
    // Note: You won't be able to use WebApplication in a class library. So you have to add this package Microsoft.AspNetCore.App
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var authRepo = scope.ServiceProvider.GetRequiredService<IAuthRepository>();

            if (dbContext.Database.GetPendingMigrations().Count() > 0)
            {
                await dbContext.Database.MigrateAsync();
            }

            if (!await dbContext.Users.AnyAsync())
            {
                // Create an user with admin role
                var user = new CreateUserDto
                {
                    FullName = "Admin",
                    Username = "admin",
                    Password = "Admin@123",
                    Role = Roles.Admin
                };
                await authRepo.SignupAsync(user);
                Console.WriteLine("====> Admin entry is created.");
            }

            // Seed Categories
            if (!await dbContext.Categories.AnyAsync())
            {
                var categories = await LoadSeedDataAsync<Category>("categories.json");
                if (categories.Any())
                {
                    await dbContext.Categories.AddRangeAsync(categories);
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine($"====> {categories.Count} categories seeded successfully.");
                }
            }

            // Seed Suppliers
            if (!await dbContext.Suppliers.AnyAsync())
            {
                var suppliers = await LoadSeedDataAsync<Supplier>("suppliers.json");
                if (suppliers.Any())
                {
                    await dbContext.Suppliers.AddRangeAsync(suppliers);
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine($"====> {suppliers.Count} suppliers seeded successfully.");
                }
            }

            // Seed Products
            if (!await dbContext.Products.AnyAsync())
            {
                var products = await LoadSeedDataAsync<Product>("products.json");
                if (products.Any())
                {
                    await dbContext.Products.AddRangeAsync(products);
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine($"====> {products.Count} products seeded successfully.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("====>" + ex.Message);
            throw;
        }
    }

    private static async Task<List<T>> LoadSeedDataAsync<T>(string fileName)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var dataPath = Path.Combine(basePath, "Data", fileName);

        // Alternative path if running from development environment
        if (!File.Exists(dataPath))
        {
            // Try to find the file relative to the project structure
            var projectPath = Directory.GetCurrentDirectory();
            var alternativePaths = new[]
                {
                    Path.Combine(projectPath, "InventoryMgt.Data", "Data", fileName),
                    Path.Combine(projectPath, "Data", fileName),
                    Path.Combine(Directory.GetParent(projectPath)?.FullName ?? "", "InventoryMgt.Data", "Data", fileName)
                };
            foreach (var altPath in alternativePaths)
            {
                if (File.Exists(altPath))
                {
                    dataPath = altPath;
                    break;
                }
            }
        }

        if (!File.Exists(dataPath))
        {
            Console.WriteLine($"====> Seed data file not found: {fileName} at {dataPath}");
            return new List<T>();
        }

        var jsonContent = await File.ReadAllTextAsync(dataPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var data = JsonSerializer.Deserialize<List<T>>(jsonContent, options);
        return data ?? new List<T>();
    }
}
