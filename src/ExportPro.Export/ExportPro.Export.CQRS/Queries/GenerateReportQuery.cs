using AutoMapper;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Enums;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.SDK.Utilities;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Refit;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace ExportPro.Export.CQRS.Queries;

public sealed record GenerateReportQuery(ReportFormat Format, ReportFilterDto Filters) : IRequest<ReportFileDto>;

public sealed class GenerateReportQueryHandler(
    IStorageServiceApi storageApi,
    IEnumerable<IReportGenerator> generators,
    IMapper mapper,
    ILogger logger
) : IRequestHandler<GenerateReportQuery, ReportFileDto>
{
    public async Task<ReportFileDto> Handle(GenerateReportQuery request, CancellationToken cancellationToken)
    {
        var reportFile = await GenerateReportFileAsync(request, cancellationToken);
        return reportFile;
    }

    private async Task<ReportFileDto> GenerateReportFileAsync(
        GenerateReportQuery request,
        CancellationToken cancellationToken
    )
    {
        logger.Information("GenerateReportFile START");
        var allInvoices = await FetchInvoicesAsync(cancellationToken);
        var clientId = request.Filters.ClientId;
        logger.Debug("ClientId: {@clientId}", clientId);
        var clientCurrencyId = request.Filters.ClientCurrencyId;
        logger.Debug("ClientCurrencyId: {@clientCurrencyId}", clientCurrencyId);
        var invoices = FilterInvoicesByClientId(clientId, allInvoices);
        logger.Debug("Invoices: {@invoices}", invoices);
        var (items, plans) = await FetchItemsAndPlansAsync(clientId, cancellationToken);

        int overdueCnt = 0;
        double? overdueAmt = null;
        if (clientId != Guid.Empty)
        {
            try
            {
                var overdueResp = await storageApi.Invoice.GetOverduePayments(
                    clientId,
                    clientCurrencyId,
                    cancellationToken
                );
                logger.Debug("OverdueResp: {@overdueResp}", overdueResp);
                if (overdueResp.IsSuccess && overdueResp.Data is not null)
                {
                    overdueCnt = overdueResp.Data.OverdueInvoicesCount;
                    overdueAmt = overdueResp.Data.TotalOverdueAmount;
                }
            }
            catch
            {
                overdueCnt = 0;
                overdueAmt = 0;
            }
        }

        var reportContent = await RetrieveClientNameAsync(request, clientId, invoices, items, plans, cancellationToken);

        var clientCurrecyCode = await storageApi.Currency.GetById(clientCurrencyId, cancellationToken);
        logger.Debug("ClientCurrecyCode: {@clientCurrecyCode}", clientCurrecyCode);
        reportContent = reportContent with
        {
            OverdueInvoicesCount = overdueCnt,
            TotalOverdueAmount = overdueAmt,
            ClientCurrencyCode = clientCurrecyCode.Data?.CurrencyCode ?? "—",
        };

        return CreateReportFileDto(reportContent, request.Format, generators);
    }

    private async Task<ReportContentDto> RetrieveClientNameAsync(
        GenerateReportQuery request,
        Guid clientId,
        List<InvoiceForReport> invoices,
        List<ItemResponse> items,
        List<PlansResponse> plans,
        CancellationToken cancellationToken
    )
    {
        var clientName = "—";

        if (clientId != Guid.Empty)
        {
            var clientResp = await storageApi.Client.GetClientById(clientId, cancellationToken);
            logger.Debug("ClientResp: {@clientResp}", clientResp);
            clientName = clientResp.Data?.Name ?? "—";
        }

        return mapper.Map<ReportContentDto>((invoices, items, plans, request.Filters, clientName));
    }

    private static List<InvoiceForReport> FilterInvoicesByClientId(Guid clientId, List<InvoiceDto> allInvoices)
    {
        var invoices =
            clientId != Guid.Empty ? allInvoices.Where(i => i.ClientId == clientId).ToList() : allInvoices.ToList();
        var invoicesForReport = invoices
            .Select(i => new InvoiceForReport
            {
                InvoiceNumber = i.InvoiceNumber,
                IssueDate = i.IssueDate,
                DueDate = i.DueDate,
                Amount = i.Amount,
                CurrencyCode = i.Currency,
                PaymentStatus = i.PaymentStatus,
                BankAccountNumber = i.BankAccountNumber,
            })
            .ToList();
        return invoicesForReport;
    }

    private async Task<List<InvoiceDto>> FetchInvoicesAsync(CancellationToken cancellationToken)
    {
        return (await storageApi.Invoice.GetInvoices(1, int.MaxValue, cancellationToken)).Data?.Items ?? [];
    }

    private async Task<(List<ItemResponse>, List<PlansResponse>)> FetchItemsAndPlansAsync(
        Guid clientId,
        CancellationToken cancellationToken
    )
    {
        if (clientId == Guid.Empty)
            return ([], []);
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 1000 };
        var itemsTask = storageApi.Client.GetClientItems(clientId, parameters, cancellationToken);
        var plansTask = storageApi.Client.GetClientPlans(clientId, parameters, cancellationToken);
        await Task.WhenAll(itemsTask, plansTask);
        var itemsResult = await itemsTask;
        var plansResult = await plansTask;
        return (itemsResult.Data?.Items?.Cast<ItemResponse>().ToList() ?? [], plansResult.Data?.Items ?? []);
    }

    private static ReportFileDto CreateReportFileDto(
        ReportContentDto dto,
        ReportFormat fmt,
        IEnumerable<IReportGenerator> generators
    )
    {
        var key = fmt.ToString();
        var generator = generators.First(g => g.Extension.Equals(key, StringComparison.OrdinalIgnoreCase));
        var bytes = generator.Generate(dto);
        var name = FileNameTemplates.CsvExcelFileName(generator.Extension);
        return new ReportFileDto(name, bytes, generator.ContentType);
    }
}
