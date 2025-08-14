using InventoryMgt.Api.Extensions;  // This line must be included
using InventoryMgt.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterServices(); // <--- updated line
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.ConfigureExceptionMiddleware();
app.MapControllers();

app.Run();
