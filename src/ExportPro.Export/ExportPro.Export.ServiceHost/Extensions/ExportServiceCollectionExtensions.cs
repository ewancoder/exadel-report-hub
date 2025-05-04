using System.Text;
using ExportPro.Common.DataAccess.MongoDB.Configurations;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.Shared.Behaviors;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Export.CQRS.Queries;
using ExportPro.Export.Pdf.Interfaces;
using ExportPro.Export.Pdf.Services;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.ServiceHost.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using Refit;

namespace ExportPro.Export.ServiceHost.Extensions;

public static class ExportServiceCollectionExtensions
{
    public static IServiceCollection AddExportModule(this IServiceCollection services, IConfiguration cfg)
    {
        // ---------- common infrastructure ----------
        services.AddCommonRegistrations();
        services.AddOpenApi();
        services.AddSwaggerServices("ExportPro Export Service");
        var jwtSettings = cfg.GetSection("JwtSettings").Get<JwtSettings>();
        services.AddAuthentication(options =>
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Secret))
    };
});

        // ---------- Mongo  ----------

        services.AddSingleton<IMongoDbConnectionFactory, MongoDbConnectionFactory>();
        services.AddSingleton<ICollectionProvider, DefaultCollectionProvider>();

        // ---------- MediatR --------------------------
        services.AddMediatR(options =>
            options.RegisterServicesFromAssemblies(
                typeof(GeneratePdfInvoiceQuery).Assembly,
                typeof(IPdfGenerator).Assembly));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // ---------- PDF ------------------------------
        services.AddSingleton<IPdfGenerator, PdfGenerator>();
        QuestPDF.Settings.License = LicenseType.Community;

        // ---------- HttpContext / auth forward -------
        services.AddHttpContextAccessor();
        services.AddTransient<ForwardAuthHeaderHandler>();

        // ---------- Refit client to Storage-service ---
        var baseUrl = Environment.GetEnvironmentVariable("StorageUrl")
                   ?? "http://localhost:5011";

        services.AddRefitClient<IStorageServiceApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<ForwardAuthHeaderHandler>();

        return services;
    }
}
