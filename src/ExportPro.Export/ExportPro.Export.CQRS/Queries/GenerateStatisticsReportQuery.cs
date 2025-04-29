using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Enums;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.SDK.Utilities;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
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
        IEnumerable<IReportGenerator> generators,
        ILogger<GenerateStatisticsReportQueryHandler> log)
    : IRequestHandler<GenerateStatisticsReportQuery, ReportFileDto>
{
    public async Task<ReportFileDto> Handle(
        GenerateStatisticsReportQuery request,
        CancellationToken cancellationToken)
    {
        log.LogInformation("Statistics export start (format={Format})", request.Format);
        var invoices = await FetchInvoicesAsync(cancellationToken);
        var (items, plans) = await FetchItemsAndPlansAsync(request.Filters.ClientId, cancellationToken);
        var dto = new StatisticsReportDto { Invoices = invoices, Items = items, Plans = plans, Filters = request.Filters };
        var file = CreateReportFile(dto, request.Format, generators);
        log.LogInformation("Statistics export done (bytes={Len})", file.Content.Length);
        return file;
    }

    private async Task<List<InvoiceDto>> FetchInvoicesAsync(CancellationToken cancellationToken)
        => (await storageApi.GetInvoicesAsync(1, int.MaxValue, false, cancellationToken))
               .Data?.Items ?? [];

    private async Task<(List<ItemResponse>, List<PlansResponse>)>
        FetchItemsAndPlansAsync(string? clientId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return ([], []);

        var itemsTask = storageApi.GetItemsByClientAsync(clientId, cancellationToken);
        var plansTask = storageApi.GetPlansByClientAsync(clientId, cancellationToken);
        await Task.WhenAll(itemsTask, plansTask);

        return (itemsTask.Result.Data ?? [],
                plansTask.Result.Data ?? []);
    }

    private static ReportFileDto CreateReportFile(StatisticsReportDto dto, ReportFormat fmt, IEnumerable<IReportGenerator> generators)
    {
        var key = fmt == ReportFormat.Csv ? "csv" : "xlsx";
        var generator = generators.First(g => g.Extension.ToLowerInvariant() == key);
        var bytes = generator.Generate(dto);
        var name = ReportFileNameTemplates.Statistics(generator.Extension);
        return new ReportFileDto(name, bytes, generator.ContentType);
    }
}
