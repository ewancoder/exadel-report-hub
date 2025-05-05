using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using ExportPro.StorageService.SDK.Services;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;


public record GetOverduePaymentsQuery(string ClientId): IQuery<OverduePaymentsResponse>;

public sealed class GetOverduePaymentsQueryHandler(IInvoiceRepository invoiceRepository, 
    ICurrencyExchangeService currencyExchangeService,
    ICurrencyRepository currencyRepository) : IQueryHandler<GetOverduePaymentsQuery, OverduePaymentsResponse>
{
    public async Task<BaseResponse<OverduePaymentsResponse>> Handle(GetOverduePaymentsQuery request, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.ClientId, out var clientObjectId))
            return new BadRequestResponse<OverduePaymentsResponse>("Invalid client ID.");

        var overdueInvoices = await invoiceRepository.GetOverdueInvoices(clientObjectId, cancellationToken);
        if (overdueInvoices == null || overdueInvoices.Count == 0)
            return new BadRequestResponse<OverduePaymentsResponse>("No invoices issued in selected period.");
        double totalAmount = 0;

        foreach (var invoice in overdueInvoices)
        {
            if (invoice.CurrencyId == invoice.ClientCurrencyId)
            {
                totalAmount += (double)invoice.Amount;
                continue;
            }

            var invoiceCurrency = await currencyRepository.GetCurrencyCodeById(invoice.CurrencyId);
            var clientCurrency = await currencyRepository.GetCurrencyCodeById(invoice.ClientCurrencyId);

            if (invoiceCurrency == null || clientCurrency == null)
                return new BadRequestResponse<OverduePaymentsResponse>("One or more currencies not found.");
            var invoiceCurrencyExchangeRateToEuro = 1.0;
            CurrencyExchangeModel currencyExchangeModel = new()
            {
                Date = invoice.IssueDate,
                From = invoiceCurrency!.CurrencyCode,
            };
            if (invoiceCurrency.CurrencyCode != "EUR")
            {
                invoiceCurrencyExchangeRateToEuro = await currencyExchangeService.ExchangeRate(currencyExchangeModel, cancellationToken);
            }
            double convertedAmount = 0;

            if (clientCurrency.CurrencyCode != "EUR")
            {
                var clientCurrencyRate = await currencyExchangeService.ExchangeRate(new CurrencyExchangeModel { Date = invoice.IssueDate, From = clientCurrency.CurrencyCode}, cancellationToken);
                convertedAmount = (double)(invoice.Amount * clientCurrencyRate);
            }
            totalAmount += convertedAmount;
        }

        var result = new OverduePaymentsResponse
        {
            OverdueInvoicesCount = overdueInvoices.Count,
            TotalOverdueAmount = (double?)totalAmount
        };
        return new SuccessResponse<OverduePaymentsResponse>(result);
    }
}

