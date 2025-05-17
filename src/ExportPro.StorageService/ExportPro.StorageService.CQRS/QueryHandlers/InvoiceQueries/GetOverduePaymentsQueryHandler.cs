using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using ExportPro.StorageService.SDK.Services;
using Serilog;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public record GetOverduePaymentsQuery(Guid ClientId, Guid ClientCurrencyId) : IQuery<OverduePaymentsResponse>;

public sealed class GetOverduePaymentsQueryHandler(
    IInvoiceRepository invoiceRepository,
    ICurrencyExchangeService currencyExchangeService,
    ICurrencyRepository currencyRepository,
    ILogger logger
) : IQueryHandler<GetOverduePaymentsQuery, OverduePaymentsResponse>
{
    public async Task<BaseResponse<OverduePaymentsResponse>> Handle(
        GetOverduePaymentsQuery request,
        CancellationToken cancellationToken
    )
    {
        logger.Information("Start handling GetOverduePaymentsQuery for ClientId: {ClientId}", request.ClientId);

        var overdueInvoices = await invoiceRepository.GetOverdueInvoices(
            request.ClientId.ToObjectId(),
            cancellationToken
        );

        if (overdueInvoices == null || overdueInvoices.Count == 0)
        {
            logger.Warning("No overdue invoices found for ClientId: {ClientId}", request.ClientId);
            return new BadRequestResponse<OverduePaymentsResponse>("No invoices issued in selected period.");
        }

        logger.Information(
            "{Count} overdue invoices found for ClientId: {ClientId}",
            overdueInvoices.Count,
            request.ClientId
        );

        double totalAmount = 0;

        foreach (var invoice in overdueInvoices)
        {
            logger.Debug(
                "Processing invoice {InvoiceId} with amount {Amount}, CurrencyId: {CurrencyId}, ClientCurrencyId: {ClientCurrencyId}",
                invoice.Id,
                invoice.Amount,
                invoice.CurrencyId,
                request.ClientCurrencyId
            );

            if (invoice.CurrencyId == request.ClientCurrencyId.ToObjectId())
            {
                totalAmount += (double)invoice.Amount!;
                logger.Debug(
                    "No currency conversion needed for invoice {InvoiceId}. Amount added: {Amount}",
                    invoice.Id,
                    invoice.Amount
                );
                continue;
            }

            var invoiceCurrency = await currencyRepository.GetCurrencyCodeById(invoice.CurrencyId);
            var clientCurrency = await currencyRepository.GetCurrencyCodeById(request.ClientCurrencyId.ToObjectId());

            if (invoiceCurrency == null || clientCurrency == null)
            {
                logger.Error(
                    "Currency not found for invoice {InvoiceId}. InvoiceCurrencyId: {InvoiceCurrencyId}, ClientCurrencyId: {ClientCurrencyId}",
                    invoice.Id,
                    invoice.CurrencyId,
                    request.ClientCurrencyId.ToObjectId()
                );
                return new BadRequestResponse<OverduePaymentsResponse>("One or more currencies not found.");
            }

            logger.Debug(
                "Invoice currency: {InvoiceCurrency}, Client currency: {ClientCurrency}",
                invoiceCurrency.CurrencyCode,
                clientCurrency.CurrencyCode
            );

            double invoiceCurrencyExchangeRateToEuro = 1.0;
            if (invoiceCurrency.CurrencyCode != "EUR")
            {
                invoiceCurrencyExchangeRateToEuro = await currencyExchangeService.ExchangeRate(
                    new CurrencyExchangeModel { Date = invoice.IssueDate, From = invoiceCurrency.CurrencyCode },
                    cancellationToken
                );
                logger.Debug(
                    "Exchange rate for invoice currency {Currency} to EUR on {Date}: {Rate}",
                    invoiceCurrency.CurrencyCode,
                    invoice.IssueDate,
                    invoiceCurrencyExchangeRateToEuro
                );
            }

            double clientCurrencyRate = 1.0;
            if (clientCurrency.CurrencyCode != "EUR")
            {
                clientCurrencyRate = await currencyExchangeService.ExchangeRate(
                    new CurrencyExchangeModel { Date = invoice.IssueDate, From = clientCurrency.CurrencyCode },
                    cancellationToken
                );
            }
            var convertedAmount = (double)(invoice.Amount! * clientCurrencyRate / invoiceCurrencyExchangeRateToEuro)!;
            logger.Debug(
                "Converted amount for invoice {InvoiceId} to client currency {ClientCurrency}: {ConvertedAmount}",
                invoice.Id,
                clientCurrency.CurrencyCode,
                convertedAmount
            );

            totalAmount += convertedAmount;
        }

        logger.Information(
            "Total overdue amount for ClientId {ClientId}: {TotalAmount}",
            request.ClientId,
            totalAmount
        );

        var result = new OverduePaymentsResponse
        {
            OverdueInvoicesCount = overdueInvoices.Count,
            TotalOverdueAmount = totalAmount,
        };

        logger.Information("Successfully handled GetOverduePaymentsQuery for ClientId: {ClientId}", request.ClientId);
        return new SuccessResponse<OverduePaymentsResponse>(result);
    }
}
