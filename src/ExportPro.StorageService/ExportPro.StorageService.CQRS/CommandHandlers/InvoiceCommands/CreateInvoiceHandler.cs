using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using ExportPro.StorageService.SDK.Services;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public sealed class CreateInvoiceCommand : ICommand<InvoiceResponse>
{
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public required Guid CurrencyId { get; set; }
    public Status? PaymentStatus { get; set; }
    public Guid CustomerId { get; set; }
    public string? BankAccountNumber { get; set; }
    public Guid ClientId { get; set; }
    public List<ItemDtoForClient>? Items { get; set; }
}

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
            InvoiceNumber = request.InvoiceNumber,
            IssueDate = request.IssueDate,
            DueDate = request.DueDate,
            CurrencyId = request.CurrencyId.ToObjectId(),
            PaymentStatus = request.PaymentStatus,
            BankAccountNumber = request.BankAccountNumber,
            ClientId = request.ClientId.ToObjectId(),
            CustomerId = request.CustomerId.ToObjectId(),
            Items = request.Items!.Select(c => mapper.Map<Item>(c)).ToList(),
        };
        foreach (var i in invoice.Items)
            i.Id = ObjectId.GenerateNewId();
        //getting the invoice currency
        var invoiceCurrency = await currencyRepository.GetCurrencyCodeById(request.CurrencyId.ToObjectId());
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
