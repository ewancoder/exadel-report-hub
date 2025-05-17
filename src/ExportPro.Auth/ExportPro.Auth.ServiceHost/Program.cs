using ExportPro.Auth.CQRS.Commands;
using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.ServiceHost.Extensions;
using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using ExportPro.Common.DataAccess.MongoDB.Contexts;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices("ExportPro Auth Service");
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IMongoDbConnectionFactory, MongoDbConnectionFactory>();
builder.Services.AddScoped<ExportProMongoContext>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

    UserRegisterDto user = new()
    {
        Email = "G10@gmail.com",
        Username = "GPPP",
        Password = "G10@gmail.com",
    };

    var userExistence = await userRepository.GetByEmailAsync(user.Email);
    
    if (userExistence == null)
    {
        var register = await authService.RegisterAsync(user);
    }
}

app.Run();
