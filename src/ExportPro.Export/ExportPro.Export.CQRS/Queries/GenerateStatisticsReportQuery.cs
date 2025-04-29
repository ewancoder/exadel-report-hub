using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Enums;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.SDK.Utilities;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExportPro.Export.CQRS.Queries;

public sealed record GenerateStatisticsReportQuery(
    ReportFormat Format,
    StatisticsFilterDto Filters)
    : IRequest<ReportFileDto>;

public sealed class GenerateStatisticsReportQueryHandler(
    IStorageServiceApi storageApi,
    IReportGeneratorResolver resolver,
    ILogger<GenerateStatisticsReportQueryHandler> logger)
    : IRequestHandler<GenerateStatisticsReportQuery, ReportFileDto>
{
    public async Task<ReportFileDto> Handle(
        GenerateStatisticsReportQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Statistics export start (format={Format})", request.Format);

        var invoicesResp = await storageApi.GetInvoicesAsync(1, 1000, false, cancellationToken);
        var invoices = invoicesResp.Data?.Items ?? [];
        var items = new List<ItemResponse>();
        var plans = new List<PlansResponse>();

        if (!string.IsNullOrWhiteSpace(request.Filters.ClientId))
        {
            items = (await storageApi.GetItemsByClientAsync(request.Filters.ClientId!, cancellationToken)).Data ?? [];
            plans = (await storageApi.GetPlansByClientAsync(request.Filters.ClientId!, cancellationToken)).Data ?? [];
        }

        StatisticsReportDto dto = new()
        {
            Invoices = invoices,
            Items = items,
            Plans = plans,
            Filters = request.Filters
        };

        var generator = resolver.Resolve(request.Format);
        var bytes = generator.Generate(dto);
        var fileName = ReportFileNameTemplates.Statistics(generator.Extension);

        logger.LogInformation("Statistics export done (bytes={Len})", bytes.Length);

        return new ReportFileDto(fileName, bytes, generator.ContentType);
    }
}
