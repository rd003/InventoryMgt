using InventoryMgt.Api.Middlewares;
using InventoryMgt.Data.Repositories;

namespace InventoryMgt.Api.Extensions;

public static class ServiceRegistratonExtension
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IPurchaseRepository, PurchaseRepository>();
        services.AddTransient<IStockRepository, StockRepository>();
        services.AddTransient<ISaleRepository, SaleRepository>();
        services.AddTransient<ISupplierRepository, SupplierRepository>();
        services.AddTransient<ExceptionMiddleware>();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("*").  // Please register specific origin, dont allow it for everyone, I am doing it deliberately.
                AllowAnyHeader().
                AllowAnyMethod().WithExposedHeaders("X-Pagination");
            });
        });
        return services;
    }
}