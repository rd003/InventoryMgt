using InventoryMgt.Data.Models;
using InventoryMgt.Data.Repositories;
using InventoryMgt.Shared.Contracts;
using InventoryMgt.Shared.Repositories;
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
        services.AddTransient<ITokenInfoRepository, TokenInfoRepository>();

        return services;
    }
}
