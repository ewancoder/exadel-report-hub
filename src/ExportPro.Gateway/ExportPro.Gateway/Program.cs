using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using ExportPro.Common.DataAccess.MongoDB.Contexts;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Gateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // Add this line
builder.Services.AddScoped<IUserRepository, UserRepository>();
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