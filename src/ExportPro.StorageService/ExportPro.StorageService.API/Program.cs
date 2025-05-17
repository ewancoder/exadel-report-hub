using System.Text;
using AutoMapper;
using ExportPro.Common.DataAccess.MongoDB.Configurations;
using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Config;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Filters;
using ExportPro.Common.Shared.Middlewares;
using ExportPro.StorageService.API;
using ExportPro.StorageService.API.Configurations;
using ExportPro.StorageService.Api.Validations.Client;
using ExportPro.StorageService.CQRS;
using ExportPro.StorageService.CQRS.Profiles;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;
using ExportPro.StorageService.SDK.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Refit;
using ExportPro.StorageService.Api.Validations.CurrencyConversion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder
    .Services.AddControllers(options =>
    {
        options.Filters.Add<ApiResponseStatusCodeFilter>();
        options.Filters.Add<PermissionFilter>();
        options.Filters.Add<ValidateModelStateAttribute>();
    })
    .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Program>());
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Host.UseSharedSerilogAndConfiguration();
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7067/"; // if using identity server or Auth0
        options.RequireHttpsMetadata = false; // optional for local dev
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings?.Issuer,
            ValidAudience = jwtSettings?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Secret!)),
        };
    });

builder
    .Services.AddRefitClient<IECBApi>(new RefitSettings { ContentSerializer = new XmlContentSerializer() })
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["Refit:appurl"] ?? string.Empty);
    });
builder.Services.AddLogging();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices("ExportPro Storage Service");
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddCommonRegistrations();
builder.Services.AddRepositoryConfig();
builder.Services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();
builder.Services.AddCQRS();
builder.Services.AddScoped<SeedingData>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<ICountryRepository>();
    var seedingData = scope.ServiceProvider.GetRequiredService<SeedingData>();
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    await seedingData.SeedCountries();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
