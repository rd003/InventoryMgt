using InventoryMgt.Api.Middlewares;
using InventoryMgt.Api.Services;

namespace InventoryMgt.Api.Extensions;

public static class ServiceRegistratonExtension
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddTransient<ExceptionMiddleware>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:4200").
                AllowCredentials().
                AllowAnyHeader().
                AllowAnyMethod().WithExposedHeaders("X-Pagination");

            });
        });
        return services;
    }
}