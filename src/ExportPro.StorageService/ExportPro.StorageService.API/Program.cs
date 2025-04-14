using System.Reflection;
using System.Text;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.AuthService.Configuration;
using ExportPro.Common.DataAccess.MongoDB.Configurations;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Middlewares;
using ExportPro.StorageService.CQRS.Commands;
using ExportPro.StorageService.CQRS.Profiles;
using ExportPro.StorageService.CQRS.Commands.Customer;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.DataAccess.Services;
using ExportPro.StorageService.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Refit;
using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.Common.DataAccess.MongoDB.Services;
var builder = WebApplication.CreateBuilder(args);


//builder.Host.UseSharedSerilogAndConfiguration();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices("ExportPro Storage Service");
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddScoped<ICollectionProvider, DefaultCollectionProvider>();
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddCommonRegistrations();
builder.Services.AddScoped<IRepository<Client>>(
    provider => provider.GetRequiredService<ClientRepository>());
builder.Services.AddScoped<IRepository<Item>>(
    provider => provider.GetRequiredService<IItemRepository>());
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<ItemRepository>();
builder.Services.AddScoped<ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IRepository<Invoice>>(
    provider => provider.GetRequiredService<IInvoiceRepository>());
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddMediatR(cfg =>
      cfg.RegisterServicesFromAssemblies(typeof(GetClientsQueryHandler).Assembly)); builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services
    .AddRefitClient<IAuth>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://authservice:8080"));
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ErrorHandlingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();