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
                policy.WithOrigins("*").  // Please register specific origin, dont allow it for everyone, I am doing it deliberately.
                AllowAnyHeader().
                AllowAnyMethod().WithExposedHeaders("X-Pagination");
            });
        });
        return services;
    }
}