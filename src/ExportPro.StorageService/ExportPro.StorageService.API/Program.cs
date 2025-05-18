using System.Text;
using ExportPro.Common.DataAccess.MongoDB.Configurations;
using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Middlewares;
using ExportPro.StorageService.API.Configurations;
using ExportPro.StorageService.CQRS;
using ExportPro.StorageService.CQRS.Profiles;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;
using ExportPro.StorageService.SDK.Services;
using ExportPro.StorageService.Validations.Validations.Client;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Refit;
using ExportPro.Common.Shared.Refit;
using System.Text.Json.Serialization;
using System.Text.Json;
using ExportPro.Export.ServiceHost.Infrastructure;
using System.Buffers.Text;
using ExportPro.StorageService.API;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var refitSettings = new RefitSettings
{
    ContentSerializer = new SystemTextJsonContentSerializer(
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        })
};
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
        c.BaseAddress = new Uri(builder.Configuration["Refit:currencyUrl"]);
    });
builder.Services.AddTransient<ForwardAuthHeaderHandler>();
builder.Services
    .AddRefitClient<IACLSharedApi>(new RefitSettings
    {
        ContentSerializer = new SystemTextJsonContentSerializer()
    })
    .ConfigureHttpClient((sp, client) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        client.BaseAddress = new Uri(config["Refit:authUrl"]);
    })
    .AddHttpMessageHandler<ForwardAuthHeaderHandler>();
builder.Services.AddLogging();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices("ExportPro Storage Service");
builder.Services.AddValidatorsFromAssembly(typeof(CreateClientCommandValidator).Assembly);
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
builder.Services.AddAutoMapper(typeof(ExportPro.StorageService.CQRS.Profiles.MappingProfile));
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddCommonRegistrations();
builder.Services.AddRepositoryConfig();
builder.Services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();
builder.Services.AddCQRS();
var app = builder.Build();
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
