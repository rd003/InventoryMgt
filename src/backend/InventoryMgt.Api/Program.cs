using InventoryMgt.Api.Extensions;  // This line must be included
using InventoryMgt.Api.Middlewares;
using InventoryMgt.Data.Utils;
using InventoryMgt.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

DapperUtil.ConfigureDapper();

// Add services to the container.
builder.Services.RegisterServices(); // <--- updated line

// connection string
string connectionString = builder.Configuration.GetConnectionString("default") ?? throw new InvalidOperationException("Connection string not found");
builder.Services.RegisterDataServices(connectionString);
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.ConfigureExceptionMiddleware();
app.MapControllers();
await app.InitializeDatabaseAsync();
app.Run();
