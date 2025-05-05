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
        ILogger<GenerateReportQueryHandler> log)
    : IRequestHandler<GenerateReportQuery, ReportFileDto>
{
    public async Task<ReportFileDto> Handle(
        GenerateReportQuery request,
        CancellationToken cancellationToken)
    {
        log.LogInformation("Statistics export start (format={Format})", request.Format);

        // 1️⃣ fetch all invoices
        var allInvoices = await FetchInvoicesAsync(cancellationToken);

        // 2️⃣ filter by the single client (if provided)
        var clientId = request.Filters.ClientId;
        var invoices = (clientId.HasValue && clientId.Value != Guid.Empty)
            ? allInvoices.Where(i => i.ClientId == clientId).ToList()
            : allInvoices;

        // 3️⃣ fetch items and plans for that client
        var (items, plans) = await FetchItemsAndPlansAsync(clientId, cancellationToken);

        // 4️⃣ look up the client name
        string clientName = "—";
        if (clientId.HasValue && clientId.Value != Guid.Empty)
        {
            var clientResp = await storageApi.GetClientByIdAsync(clientId.Value, cancellationToken);
            clientName = clientResp.Data?.Name ?? "—";
        }

        // 5️⃣ build the report DTO, including the single client’s name
        var dto = new ReportContentDto
        {
            Invoices   = invoices,
            Items      = items,
            Plans      = plans,
            Filters    = request.Filters,
            ClientName = clientName
        };

        // 6️⃣ generate the file
        var file = CreateReportFile(dto, request.Format, generators);

        log.LogInformation("Statistics export done (bytes={Len})", file.Content.Length);
        return file;
    }

    private async Task<List<InvoiceDto>> FetchInvoicesAsync(CancellationToken cancellationToken)
        => (await storageApi.GetInvoicesAsync(1, int.MaxValue, false, cancellationToken))
               .Data?.Items ?? [];

    private async Task<(List<ItemResponse>, List<PlansResponse>)>
        FetchItemsAndPlansAsync(Guid? clientId, CancellationToken cancellationToken)
    {
        if (clientId == null || clientId == Guid.Empty)
            return (new(), new());

        var itemsTask = storageApi.GetItemsByClientAsync(clientId.Value, cancellationToken);
        var plansTask = storageApi.GetPlansByClientAsync(clientId.Value, cancellationToken);
        await Task.WhenAll(itemsTask, plansTask);

        return (itemsTask.Result.Data ?? new(), plansTask.Result.Data ?? new());
    }

    private static ReportFileDto CreateReportFile(
        ReportContentDto dto,
        ReportFormat fmt,
        IEnumerable<IReportGenerator> generators)
    {
        var key       = fmt == ReportFormat.Csv ? "csv" : "xlsx";
        var generator = generators.First(g => g.Extension.Equals(key, StringComparison.OrdinalIgnoreCase));
        var bytes     = generator.Generate(dto);
        var name      = FileNameTemplates.CsvExcelFileName(generator.Extension);
        return new ReportFileDto(name, bytes, generator.ContentType);
    }
}
