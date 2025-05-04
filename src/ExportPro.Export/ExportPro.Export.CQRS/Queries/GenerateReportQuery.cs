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

/// <summary>Generates CSV/XLSX statistics reports.</summary>
public sealed class GenerateReportQueryHandler(
        IStorageServiceApi                storageApi,
        IEnumerable<IReportGenerator>     generators,
        ILogger<GenerateReportQueryHandler> log)
    : IRequestHandler<GenerateReportQuery, ReportFileDto>
{
    public async Task<ReportFileDto> Handle(
        GenerateReportQuery request,
        CancellationToken   cancellationToken)
    {
        log.LogInformation("Statistics export start (format={Format})", request.Format);

        var allInvoices = await FetchInvoicesAsync(cancellationToken);

        List<InvoiceDto> invoices =
            request.Filters.ClientIds?.Any() == true
                ? allInvoices.Where(i => i.ClientId.HasValue &&
                                          request.Filters.ClientIds!.Contains(i.ClientId.Value))
                             .ToList()
            : request.Filters.ClientId is { } single && single != Guid.Empty
                ? allInvoices.Where(i => i.ClientId == single).ToList()
                : allInvoices;

        if (request.Filters.IssueDateFrom is DateTime d)
            invoices = invoices.Where(i => i.IssueDate.Date >= d.Date).ToList();

        var (itemsByClient, plansByClient, namesByClient) =
            await FetchPerClientDataAsync(request.Filters, cancellationToken);

        var dto = new ReportContentDto
        {
            Invoices       = invoices,
            Items          = itemsByClient.Values.SelectMany(x => x).ToList(),
            Plans          = plansByClient.Values.SelectMany(x => x).ToList(),
            ItemsByClient  = itemsByClient,
            PlansByClient  = plansByClient,
            ClientNames    = namesByClient,
            Filters        = request.Filters
        };

        var file = CreateReportFile(dto, request.Format, generators);
        log.LogInformation("Statistics export done (bytes={Len})", file.Content.Length);
        return file;
    }
    
    private async Task<List<InvoiceDto>> FetchInvoicesAsync(CancellationToken ct)
    {
        var resp = await storageApi.GetInvoicesAsync(1, int.MaxValue, false, ct);
        return resp.Data?.Items ?? [];
    }

    private async Task<(Dictionary<Guid,List<ItemResponse>>,
                        Dictionary<Guid,List<PlansResponse>>,
                        Dictionary<Guid,string>)>
            FetchPerClientDataAsync(ReportFilterDto f, CancellationToken ct)
    {
        var ids = f.ClientIds?.Any() == true
            ? f.ClientIds!
            : f.ClientId is { } single && single != Guid.Empty
                ? [ single ]
                : [];

        var itemsBy  = new Dictionary<Guid, List<ItemResponse>>();
        var plansBy  = new Dictionary<Guid, List<PlansResponse>>();
        var namesBy  = new Dictionary<Guid, string>();

        var tasks = ids.Select(async id =>
        {
            var itemsTask  = storageApi.GetItemsByClientAsync(id, ct);
            var plansTask  = storageApi.GetPlansByClientAsync(id, ct);
            var nameTask   = storageApi.GetClientByIdAsync(id, ct);

            await Task.WhenAll(itemsTask, plansTask, nameTask);

            return (id,
                    itemsTask.Result.Data ?? [],
                    plansTask.Result.Data ?? [],
                    nameTask.Result.Data?.Name ?? "—");
        });

        foreach (var t in await Task.WhenAll(tasks))
        {
            itemsBy[t.id] = t.Item2;
            plansBy[t.id] = t.Item3;
            namesBy[t.id] = t.Item4;
        }

        return (itemsBy, plansBy, namesBy);
    }

    private static ReportFileDto CreateReportFile(
        ReportContentDto              dto,
        ReportFormat                  fmt,
        IEnumerable<IReportGenerator> generators)
    {
        var key       = fmt == ReportFormat.Csv ? "csv" : "xlsx";
        var generator = generators.First(g => g.Extension.Equals(key, StringComparison.OrdinalIgnoreCase));
        var bytes     = generator.Generate(dto);
        var name      = FileNameTemplates.CsvExcelFileName(generator.Extension);
        return new ReportFileDto(name, bytes, generator.ContentType);
    }
}