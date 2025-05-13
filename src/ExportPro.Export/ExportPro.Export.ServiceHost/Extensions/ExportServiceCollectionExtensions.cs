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
        // —— common / swagger ——
        services.AddCommonRegistrations();
        services.AddOpenApi();
        services.AddSwaggerServices("ExportPro Export Service");

        // —— auth ——
        var jwt = cfg.GetSection("JwtSettings").Get<JwtSettings>();
        services
            .AddAuthentication(o => o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.Authority = "https://localhost:7067/";
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

        // —— Mongo ——
        services.AddSingleton<IMongoDbConnectionFactory, MongoDbConnectionFactory>();
        services.AddSingleton<ICollectionProvider, DefaultCollectionProvider>();

        // // —— MediatR ——
        // services.AddMediatR(o =>
        // {
        //     o.RegisterServicesFromAssemblies(typeof(GenerateInvoicePdfQuery).Assembly, typeof(IPdfGenerator).Assembly);
        //     o.AddOpenBehavior(typeof(ExportLoggingBehavior<,>));
        //     o.AddOpenBehavior(typeof(ValidationBehavior<,>));
        // });
        
        // —— MediatR —— 
        services.AddMediatR(o => o.RegisterServicesFromAssemblies(
            typeof(GenerateInvoicePdfQuery).Assembly,
            typeof(IPdfGenerator).Assembly));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        
        // —— AutoMapper ——
        services.AddAutoMapper(typeof(MappingProfile));

        // —— PDF ——
        services.AddSingleton<IPdfGenerator, PdfGenerator>();
        Settings.License = LicenseType.Community;

        // —— CSV / XLSX generators ——
        services.AddSingleton<IReportGenerator, CsvReportGenerator>();
        services.AddSingleton<IReportGenerator, ExcelReportGenerator>();
        services.AddSingleton<ICustomerExcelParser, CustomerExcelParser>();

        // —— HttpContext / auth forwarding ——
        services.AddHttpContextAccessor();
        services.AddTransient<ForwardAuthHeaderHandler>();

        // —— Refit client to Storage-service ——
        var baseUrl = Environment.GetEnvironmentVariable("StorageUrl") ?? "http://localhost:5011";
        Console.WriteLine(baseUrl);
        services
            .AddRefitClient<IStorageServiceApi>(
                new RefitSettings
                {
                    ContentSerializer = new NewtonsoftJsonContentSerializer(
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                        }
                    ),
                }
            )
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<ForwardAuthHeaderHandler>();

        return services;
    }
}
