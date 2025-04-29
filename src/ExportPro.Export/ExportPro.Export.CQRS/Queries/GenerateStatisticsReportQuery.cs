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
        IReportGeneratorResolver resolver,
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
        var dto = BuildStatisticsDto(invoices, items, plans, request.Filters);
        var file = CreateReportFile(dto, request.Format);
        log.LogInformation("Statistics export done (bytes={Len})", file.Content.Length);
        return file;
    }

    private async Task<List<InvoiceDto>> FetchInvoicesAsync(CancellationToken cancellationToken)
        => (await storageApi.GetInvoicesAsync(1, int.MaxValue, false, cancellationToken))
              .Data?.Items ?? [];

    private async Task<(List<ItemResponse> Items, List<PlansResponse> Plans)>
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

    private static StatisticsReportDto BuildStatisticsDto(
        List<InvoiceDto> invoices,
        List<ItemResponse> items,
        List<PlansResponse> plans,
        StatisticsFilterDto filters)
        => new()
        {
            Invoices = invoices,
            Items = items,
            Plans = plans,
            Filters = filters
        };

    private ReportFileDto CreateReportFile(
        StatisticsReportDto dto,
        ReportFormat format)
    {
        var generator = resolver.Resolve(format);
        var bytes = generator.Generate(dto);
        var name = ReportFileNameTemplates.Statistics(generator.Extension);
        return new ReportFileDto(name, bytes, generator.ContentType);
    }
}
