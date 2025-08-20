using InventoryMgt.Data.models;
using InventoryMgt.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryMgt.Data.Extensions;

public static class RegisterDataServicesExtension
{
    public static IServiceCollection RegisterDataServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString));

        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IPurchaseRepository, PurchaseRepository>();
        services.AddTransient<IStockRepository, StockRepository>();
        services.AddTransient<ISaleRepository, SaleRepository>();
        services.AddTransient<ISupplierRepository, SupplierRepository>();
        services.AddTransient<IAuthRepository, AuthRepository>();

        return services;
    }
}
