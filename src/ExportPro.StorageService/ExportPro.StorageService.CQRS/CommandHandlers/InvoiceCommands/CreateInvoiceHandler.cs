using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;
using ExportPro.StorageService.SDK.Services;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public sealed record CreateInvoiceCommand(CreateInvoiceDto CreateInvoiceDto) : ICommand<InvoiceResponse>;

public sealed class CreateInvoiceHandler(
    IInvoiceRepository repository,
    IMapper mapper,
    ICurrencyExchangeService currencyExchangeService,
    ICurrencyRepository currencyRepository,
    //i need to use this manually because it is not validating automatically
    IValidator<CurrencyExchangeModel> validator
) : ICommandHandler<CreateInvoiceCommand, InvoiceResponse>
{
    public async Task<BaseResponse<InvoiceResponse>> Handle(
        CreateInvoiceCommand request,
        CancellationToken cancellationToken
    )
    {
        var invoice = new Invoice
        {
            Id = ObjectId.GenerateNewId(),
            InvoiceNumber = request.CreateInvoiceDto.InvoiceNumber,
            IssueDate = request.CreateInvoiceDto.IssueDate,
            DueDate = request.CreateInvoiceDto.DueDate,
            CurrencyId = request.CreateInvoiceDto.CurrencyId.ToObjectId(),
            PaymentStatus = request.CreateInvoiceDto.PaymentStatus,
            BankAccountNumber = request.CreateInvoiceDto.BankAccountNumber,
            ClientId = request.CreateInvoiceDto.ClientId.ToObjectId(),
            CustomerId = request.CreateInvoiceDto.CustomerId.ToObjectId(),
            ClientCurrencyId = request.CreateInvoiceDto.ClientCurrencyId.ToObjectId(),
            Items = request.CreateInvoiceDto.Items!.Select(c => mapper.Map<Item>(c)).ToList(),
        };
        foreach (var i in invoice.Items)
            i.Id = ObjectId.GenerateNewId();
        //getting the invoice currency
        var invoiceCurrency = await currencyRepository.GetCurrencyCodeById(
            request.CreateInvoiceDto.CurrencyId.ToObjectId()
        );
        CurrencyExchangeModel currencyExchangeModel = new()
        {
            Date = invoice.IssueDate,
            From = invoiceCurrency!.CurrencyCode,
        };
        //converting the invoice's currency to euro becuase the api converts the currency to EUR only
        //if it is already EUR than there is no need to convert it
        var invoiceCurrencyExchangeRateToEuro = 1.0;
        if (invoiceCurrency.CurrencyCode != "EUR")
        {
            //manually validating because it is not validating automatically and catching it in the middlewere.
            await validator.ValidateAndThrowAsync(currencyExchangeModel, cancellationToken);
            invoiceCurrencyExchangeRateToEuro = await currencyExchangeService.ExchangeRate(currencyExchangeModel);
        }

        invoice.Amount = 0;
        //going to convert items currency to invoice's currency
        //first converting to euro
        foreach (var i in invoice.Items)
        {
            var currency = await currencyRepository.GetCurrencyCodeById(i.CurrencyId);
            if (currency == null)
                return new BadRequestResponse<InvoiceResponse> { Messages = ["Currency not found."] };
            //item's currencycode
            var currencyCode = currency.CurrencyCode;
            //converting item currency to EUR
            var itemExchangeRateToEuro = 1.0;
            if (currencyCode != "EUR" && invoiceCurrency.CurrencyCode != currencyCode)
            {
                currencyExchangeModel.From = currencyCode;
                await validator.ValidateAndThrowAsync(currencyExchangeModel, cancellationToken);
                //getting the exchange rate of item's currency code and EUR
                itemExchangeRateToEuro = await currencyExchangeService.ExchangeRate(
                    currencyExchangeModel,
                    cancellationToken
                );
            }

            //converting and getting the amount
            var amount = i.Price * invoiceCurrencyExchangeRateToEuro / itemExchangeRateToEuro;
            invoice.Amount += amount;
        }

        await repository.AddOneAsync(invoice, cancellationToken);
        var invoiceResponse = mapper.Map<InvoiceResponse>(invoice);
        return new SuccessResponse<InvoiceResponse>(invoiceResponse, "Invoice created successfully.");
    }
}
