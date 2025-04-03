using ExportPro.Gateway.Extensions;
using ExportPro.AuthService.Services;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.AuthService.Repositories;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.DataAccess.MongoDB.Contexts;
using ExportPro.AuthService.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add controllers, Swagger, etc.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices();

// Configure JwtSettings from configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Register services and repositories
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register MongoDB dependencies
builder.Services.AddSingleton<IMongoDbConnectionFactory, MongoDbConnectionFactory>();
builder.Services.AddScoped<ExportProMongoContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
