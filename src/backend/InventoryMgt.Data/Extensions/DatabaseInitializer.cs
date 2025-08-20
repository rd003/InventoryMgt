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

            if(dbContext.Database.GetPendingMigrations().Count()>0)
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
            // TODO: Seed Category,product and suppliers too
        }
        catch (Exception ex)
        {
            Console.WriteLine("====>" + ex.Message);
            throw;
        }
    }
}
