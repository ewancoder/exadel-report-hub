﻿using System.Net;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Services;
using FluentValidation;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public sealed record GetTotalRevenueQuery(TotalRevenueDto RevenueDto) : IQuery<double>;

public sealed class GetTotalRevenueHandler(
    IInvoiceRepository invoiceRepository,
    ICurrencyRepository currencyRepository,
    ICurrencyExchangeService exchangeService,
    IValidator<CurrencyExchangeModel> validator
) : IQueryHandler<GetTotalRevenueQuery, double>
{
    public async Task<BaseResponse<double>> Handle(GetTotalRevenueQuery request, CancellationToken cancellationToken)
    {
        var invoices = await invoiceRepository.GetInvoicesInDateRangeAsync(
            request.RevenueDto.StartDate,
            request.RevenueDto.EndDate
        );

        if (invoices == null || invoices.Count == 0)
            return new BaseResponse<double>
            {
                Data = 0,
                IsSuccess = true,
                ApiState = HttpStatusCode.OK,
                Messages = ["No invoices issued in selected period."],
            };

        double totalInClientCurrency = 0;

        foreach (var invoice in invoices)
        {
            var invoiceCurrency = await currencyRepository.GetCurrencyCodeById(invoice.CurrencyId);
            var clientCurrency = await currencyRepository.GetCurrencyCodeById(
                request.RevenueDto.ClientCurrencyId.ToObjectId()
            );

            if (invoiceCurrency == null || clientCurrency == null)
                return new BadRequestResponse<double> { Messages = ["Currency not found."] };

            var invoiceCurrencyCode = invoiceCurrency.CurrencyCode;
            var clientCurrencyCode = clientCurrency.CurrencyCode;

            var amount = invoice.Amount ?? 0;

            if (invoiceCurrencyCode == clientCurrencyCode)
            {
                totalInClientCurrency += amount;
                continue;
            }

            var invoiceToEuro = 1.0;
            if (invoiceCurrencyCode != "EUR")
            {
                var toEuroModel = new CurrencyExchangeModel { Date = invoice.IssueDate, From = invoiceCurrencyCode };
                await validator.ValidateAndThrowAsync(toEuroModel, cancellationToken);
                invoiceToEuro = await exchangeService.ExchangeRate(toEuroModel, cancellationToken);
                if (invoiceToEuro == 0)
                    return new BadRequestResponse<double>
                    {
                        Messages = [$"Currency {invoiceCurrencyCode} is not supported by ECB."],
                    };
            }

            var euroToClient = 1.0;
            if (clientCurrencyCode != "EUR")
            {
                var fromEuroModel = new CurrencyExchangeModel { Date = invoice.IssueDate, From = clientCurrencyCode };
                await validator.ValidateAndThrowAsync(fromEuroModel, cancellationToken);
                var clientToEuro = await exchangeService.ExchangeRate(fromEuroModel, cancellationToken);
                if (clientToEuro == 0)
                    return new BadRequestResponse<double>
                    {
                        Messages = [$"Currency {clientCurrencyCode} is not supported by ECB."],
                    };
                euroToClient = 1 / clientToEuro;
            }

            var converted = amount * (1 / invoiceToEuro) * euroToClient;
            totalInClientCurrency += converted;
        }

        return new BaseResponse<double>
        {
            Data = totalInClientCurrency,
            IsSuccess = true,
            ApiState = HttpStatusCode.OK,
            Messages = ["Total revenue calculated successfully."],
        };
    }
}
