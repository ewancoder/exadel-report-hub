using System.Security.Claims;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Enums;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.SDK.Utilities;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExportPro.Export.CQRS.Queries;

public sealed record GenerateReportQuery(
    ReportFormat Format,
    ReportFilterDto Filters)
    : IRequest<ReportFileDto>;

public sealed class GenerateReportQueryHandler(
    IStorageServiceApi storageApi,
    IEnumerable<IReportGenerator> generators,
    ILogger<GenerateReportQueryHandler> log,
    IHttpContextAccessor httpContext)
    : IRequestHandler<GenerateReportQuery, ReportFileDto>
{
    public async Task<ReportFileDto> Handle(
        GenerateReportQuery request,
        CancellationToken cancellationToken)
    {
        log.LogInformation("Statistics export start (format={Format})", request.Format);

        var allInvoices = await FetchInvoicesAsync(cancellationToken);
        List<InvoiceDto> invoices = request.Filters.ClientIds?.Any() == true
            ? allInvoices.Where(i => i.ClientId.HasValue && request.Filters.ClientIds!.Contains(i.ClientId.Value)).ToList()
            : request.Filters.ClientId is { } cid && cid != Guid.Empty
                ? allInvoices.Where(i => i.ClientId == cid).ToList()
                : allInvoices;

        if (request.Filters.IssueDateFrom is DateTime d)
        {
            var dayStart = d.Date;
            invoices = invoices
                .Where(i => i.IssueDate.Date >= dayStart)
                .ToList();
        }

        var (itemsByClient, plansByClient) =
            await FetchItemsAndPlansAsync(request.Filters, cancellationToken);

        var dto = new ReportContentDto
        {
            Invoices = invoices,
            Items = itemsByClient.Values.SelectMany(x => x).ToList(),
            Plans = plansByClient.Values.SelectMany(x => x).ToList(),
            ItemsByClient = itemsByClient,
            PlansByClient = plansByClient,
            Filters = request.Filters
        };

        var file = CreateReportFile(dto, request.Format, generators);
        log.LogInformation("Statistics export done (bytes={Len})", file.Content.Length);
        return file;
    }

    private async Task<(Dictionary<Guid, List<ItemResponse>>, Dictionary<Guid, List<PlansResponse>>)>
        FetchItemsAndPlansAsync(ReportFilterDto f, CancellationToken ct)
    {
        var ids = f.ClientIds?.Any() == true
            ? f.ClientIds!
            : f.ClientId is { } single && single != Guid.Empty
                ? [single]
                : [];

        var itemsByClient = new Dictionary<Guid, List<ItemResponse>>();
        var plansByClient = new Dictionary<Guid, List<PlansResponse>>();

        var tasks = ids.Select(async id =>
        {
            var items = await storageApi.GetItemsByClientAsync(id, ct);
            var plans = await storageApi.GetPlansByClientAsync(id, ct);
            return (id,
                items.Data ?? [],
                plans.Data ?? []);
        });

        foreach (var t in await Task.WhenAll(tasks))
        {
            itemsByClient[t.id] = t.Item2;
            plansByClient[t.id] = t.Item3;
        }

        return (itemsByClient, plansByClient);
    }

    private async Task<(List<ItemResponse>, List<PlansResponse>)>
        FetchItemsAndPlansAsync(Guid? clientId, CancellationToken cancellationToken)
    {
        if (clientId == null || clientId == Guid.Empty)
            return ([], []);

        var itemsTask = storageApi.GetItemsByClientAsync(clientId.Value, cancellationToken);
        var plansTask = storageApi.GetPlansByClientAsync(clientId.Value, cancellationToken);
        await Task.WhenAll(itemsTask, plansTask);

        return (itemsTask.Result.Data ?? [],
            plansTask.Result.Data ?? []);
    }

    private async Task<List<InvoiceDto>> FetchInvoicesAsync(CancellationToken ct)
    {
        var authHeader = httpContext.HttpContext?.Request.Headers["Authorization"].ToString();
        var userId = httpContext.HttpContext?.User?
                         .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        var resp = await storageApi.GetInvoicesAsync(1, int.MaxValue, false, ct);
        return resp.Data?.Items ?? [];
    }

    private static ReportFileDto CreateReportFile(
        ReportContentDto dto,
        ReportFormat fmt,
        IEnumerable<IReportGenerator> generators)
    {
        var key = fmt == ReportFormat.Csv ? "csv" : "xlsx";
        var generator = generators.First(g => g.Extension.ToLowerInvariant() == key);
        var bytes = generator.Generate(dto);
        var name = FileNameTemplates.CsvExcelFileName(generator.Extension);
        return new ReportFileDto(name, bytes, generator.ContentType);
    }
}