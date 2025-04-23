using ExportPro.Export.CQRS.Queries;
using ExportPro.Export.Pdf.Interfaces;
using ExportPro.Export.Pdf.Services;
using ExportPro.Export.SDK.Interfaces;
using Refit;

namespace ExportPro.Export.ServiceHost.Extensions;

public static class ExportServiceCollectionExtensions
{
    public static IServiceCollection AddExportModule(this IServiceCollection services, IConfiguration cfg)
    {
        //services.AddMediatR(opt => opt.RegisterServicesFromAssemblyContaining<IPdfGenerator>());
        services.AddMediatR(opt => 
            opt.RegisterServicesFromAssemblies(
                typeof(GeneratePdfInvoiceQuery).Assembly,
                typeof(IPdfGenerator).Assembly));

    https://localhost:7195

        services.AddSingleton<IPdfGenerator, PdfGenerator>();

        //services.AddRefitClient<IStorageServiceApi>()
        //        .ConfigureHttpClient(c =>
        //            c.BaseAddress = new Uri(cfg["StorageService:BaseUrl"]));

        var baseUrl = cfg["StorageService:BaseUrl"]
              ?? throw new InvalidOperationException("StorageService:BaseUrl config missing.");

        services.AddRefitClient<IStorageServiceApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        return services;
    }
}
