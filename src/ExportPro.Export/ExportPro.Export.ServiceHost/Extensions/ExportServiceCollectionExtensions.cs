using ExportPro.Export.CQRS.Queries;
using ExportPro.Export.Pdf.Interfaces;
using ExportPro.Export.Pdf.Services;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.ServiceHost.Infrastructure;
using Refit;

namespace ExportPro.Export.ServiceHost.Extensions;

public static class ExportServiceCollectionExtensions
{
    public static IServiceCollection AddExportModule(
        this IServiceCollection services, IConfiguration cfg)
    {
        // ---------- MediatR ----------
        services.AddMediatR(options =>
            options.RegisterServicesFromAssemblies(
                typeof(GeneratePdfInvoiceQuery).Assembly,
                typeof(IPdfGenerator).Assembly));

        // ---------- PDF ----------
        services.AddSingleton<IPdfGenerator, PdfGenerator>();

        // ---------- HttpContext accessor ----------
        services.AddHttpContextAccessor();
        services.AddTransient<ForwardAuthHeaderHandler>();

        // ---------- Refit client ----------
        var baseUrl = cfg.GetValue<string>("StorageService:BaseUrl")
                     ?? "http://localhost:5011";

        services.AddRefitClient<IStorageServiceApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<ForwardAuthHeaderHandler>();

        return services;
    }
}
