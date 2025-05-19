using System.Text;
using ExportPro.Common.DataAccess.MongoDB.Configurations;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Export.CQRS.Behaviors;
using ExportPro.Export.CQRS.Profile;
using ExportPro.Export.CQRS.Queries;
using ExportPro.Export.Csv.Services;
using ExportPro.Export.Excel.Services;
using ExportPro.Export.Pdf.Interfaces;
using ExportPro.Export.Pdf.Services;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.ServiceHost.Infrastructure;
using ExportPro.StorageService.SDK.Refit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuestPDF;
using QuestPDF.Infrastructure;
using Refit;

namespace ExportPro.Export.ServiceHost.Extensions;

public static class ExportServiceCollectionExtensions
{
    public static IServiceCollection AddExportModule(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddCommonRegistrations();
        ConfigureSwaggerServices(services);
        ConfigureJwtAuthentication(services, cfg);
        ConfigureMongoServices(services);
        RegisterExportServices(services);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddAutoMapper(typeof(MappingProfile));
        ConfigurePdfGeneration(services);
        ConfigureReportServices(services);
        services.AddHttpContextAccessor();
        services.AddTransient<ForwardAuthHeaderHandler>();
        ConfigureStorageServiceClient(services);
        return services;
    }

    private static void ConfigureStorageServiceClient(IServiceCollection services)
    {
        services.AddSingleton<IStorageServiceApi>(sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var baseUrl = Environment.GetEnvironmentVariable("StorageUrl") ?? "http://localhost:5011";
            var authHandler = new ForwardAuthHeaderHandler(httpContextAccessor)
            {
                InnerHandler = new HttpClientHandler(),
            };
            var http = new HttpClient(authHandler) { BaseAddress = new Uri(baseUrl) };
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    }
                ),
            };
            return new StorageServiceApi(http, refitSettings);
        });
    }

    private static void ConfigureReportServices(IServiceCollection services)
    {
        services.AddSingleton<IReportGenerator, CsvReportGenerator>();
        services.AddSingleton<IReportGenerator, ExcelReportGenerator>();
        services.AddSingleton<ICustomerExcelParser, CustomerExcelParser>();
    }

    private static void ConfigurePdfGeneration(IServiceCollection services)
    {
        services.AddSingleton<IPdfGenerator, PdfGenerator>();
        Settings.License = LicenseType.Community;
    }

    private static void RegisterExportServices(IServiceCollection services)
    {
        services.AddMediatR(o =>
        {
            o.RegisterServicesFromAssemblies(typeof(GenerateInvoicePdfQuery).Assembly, typeof(IPdfGenerator).Assembly);
            o.AddOpenBehavior(typeof(ExportLoggingBehavior<,>));
            o.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
    }

    private static void ConfigureMongoServices(IServiceCollection services)
    {
        services.AddSingleton<IMongoDbConnectionFactory, MongoDbConnectionFactory>();
        services.AddSingleton<ICollectionProvider, DefaultCollectionProvider>();
    }

    private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration cfg)
    {
        var jwt = cfg.GetSection("JwtSettings").Get<JwtSettings>();
        services
            .AddAuthentication(o => o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.Authority = "http://authservice:8080/";
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt?.Issuer,
                    ValidAudience = jwt?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt!.Secret)),
                };
            });
    }

    private static void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddSwaggerServices("ExportPro Export Service");
    }
}
