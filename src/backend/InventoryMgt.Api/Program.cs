using InventoryMgt.Api.Extensions;  // This line must be included
using InventoryMgt.Api.Middlewares;
using InventoryMgt.Data.Utils;
using InventoryMgt.Data.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

DapperUtil.ConfigureDapper();

// TODO: These settings can be extracted out from here
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:secret"]))
    };
});


// Add services to the container.
builder.Services.RegisterServices(); // <--- updated line

// connection string
string connectionString = builder.Configuration.GetConnectionString("default") ?? throw new InvalidOperationException("Connection string not found");
builder.Services.RegisterDataServices(connectionString);
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.ConfigureExceptionMiddleware();
app.MapControllers();
await app.InitializeDatabaseAsync();
app.Run();
