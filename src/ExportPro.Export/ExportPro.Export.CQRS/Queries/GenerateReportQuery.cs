using AutoMapper;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Enums;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.SDK.Utilities;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExportPro.Export.CQRS.Queries;

public sealed record GenerateReportQuery(
    ReportFormat Format,
    ReportFilterDto Filters)
    : IRequest<ReportFileDto>;

public sealed class GenerateReportQueryHandler(
    IStorageServiceApi storageApi,
    IEnumerable<IReportGenerator> generators,
    IMapper mapper,
    ILogger<GenerateReportQueryHandler> log)
    : IRequestHandler<GenerateReportQuery, ReportFileDto>
{
    public async Task<ReportFileDto> Handle(
        GenerateReportQuery request,
        CancellationToken cancellationToken)
    {
        log.LogInformation("Statistics export start (format={Format})", request.Format);
        var reportFile = await GenerateReportFileAsync(request, cancellationToken);
        log.LogInformation("Statistics export done (bytes={Len})", reportFile.Content.Length);
        return reportFile;
    }

    private async Task<ReportFileDto> GenerateReportFileAsync(GenerateReportQuery request,
        CancellationToken cancellationToken)
    {
        var allInvoices = await FetchInvoicesAsync(cancellationToken);
        var clientId = request.Filters.ClientId;
        var invoices = FilterInvoicesByClientId(clientId, allInvoices);
        var (items, plans) = await FetchItemsAndPlansAsync(clientId, cancellationToken);
        var reportContentDto =
            await RetrieveClientNameAsync(request, clientId, invoices, items, plans, cancellationToken);
        var reportFileDto = CreateReportFileDto(reportContentDto, request.Format, generators);
        return reportFileDto;
    }

    private async Task<ReportContentDto> RetrieveClientNameAsync(
        GenerateReportQuery request,
        Guid clientId,
        List<InvoiceDto> invoices,
        List<ItemResponse> items,
        List<PlansResponse> plans,
        CancellationToken cancellationToken)
    {
        var clientName = "—";

        if (clientId != Guid.Empty)
        {
            var clientResp = await storageApi.GetClientByIdAsync(clientId, cancellationToken);
            clientName = clientResp.Data?.Name ?? "—";
        }

        return mapper.Map<ReportContentDto>((invoices, items, plans, request.Filters, clientName));
    }

    private static List<InvoiceDto> FilterInvoicesByClientId(Guid clientId, List<InvoiceDto> allInvoices)
    {
        var invoices = clientId != Guid.Empty
            ? allInvoices.Where(i => i.ClientId == clientId).ToList()
            : allInvoices;
        return invoices;
    }

    private async Task<List<InvoiceDto>> FetchInvoicesAsync(CancellationToken cancellationToken)
    {
        return (await storageApi.GetInvoicesAsync(1, int.MaxValue, false, cancellationToken))
            .Data?.Items ?? [];
    }

    private async Task<(List<ItemResponse>, List<PlansResponse>)>
        FetchItemsAndPlansAsync(Guid clientId, CancellationToken cancellationToken)
    {
        if (clientId == Guid.Empty)
            return (new List<ItemResponse>(), new List<PlansResponse>());

        var itemsTask = storageApi.GetItemsByClientAsync(clientId, cancellationToken);
        var plansTask = storageApi.GetPlansByClientAsync(clientId, cancellationToken);
        await Task.WhenAll(itemsTask, plansTask);

        return (itemsTask.Result.Data ?? new List<ItemResponse>(), plansTask.Result.Data ?? new List<PlansResponse>());
    }

    private static ReportFileDto CreateReportFileDto(
        ReportContentDto dto,
        ReportFormat fmt,
        IEnumerable<IReportGenerator> generators)
    {
        var key = fmt == ReportFormat.Csv ? "csv" : "xlsx";
        var generator = generators.First(g => g.Extension.Equals(key, StringComparison.OrdinalIgnoreCase));
        var bytes = generator.Generate(dto);
        var name = FileNameTemplates.CsvExcelFileName(generator.Extension);
        return new ReportFileDto(name, bytes, generator.ContentType);
    }
}