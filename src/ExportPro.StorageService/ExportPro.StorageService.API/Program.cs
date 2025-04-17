using ExportPro.Common.DataAccess.MongoDB.Configurations;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Middlewares;
using ExportPro.StorageService.CQRS;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;
using ExportPro.StorageService.SDK.Services;
using ExportPro.StorageService.Validations.Validations.Client;
using FluentValidation;
using MediatR;
using Refit;
var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSharedSerilogAndConfiguration();
// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .AddRefitClient<IECBApi>(
    
       new RefitSettings {
        ContentSerializer = new XmlContentSerializer()
    
    })
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["Refit:appurl"]);
    });
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices("ExportPro Storage Service");
builder.Services.AddValidatorsFromAssembly(typeof(CreateClientCommandValidator).Assembly);
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddAutoMapper(typeof(ExportPro.StorageService.CQRS.Profiles.MappingProfile));
builder.Services.AddCommonRegistrations();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IRepository<Invoice>>(provider => provider.GetRequiredService<IInvoiceRepository>());
builder.Services.AddScoped<ICollectionProvider, DefaultCollectionProvider>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICurrencyExchangeService,CurrencyExchangeService>();
builder.Services.AddCQRS();
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
