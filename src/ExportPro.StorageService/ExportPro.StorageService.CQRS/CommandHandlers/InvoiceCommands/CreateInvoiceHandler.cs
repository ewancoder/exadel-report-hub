using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;
using ExportPro.StorageService.SDK.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Serilog;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public sealed record CreateInvoiceCommand(CreateInvoiceDto CreateInvoiceDto)
    : ICommand<InvoiceDto>,
        IPermissionedRequest
{
    public List<Guid>? ClientIds { get; init; } = [CreateInvoiceDto.ClientId];
    public Resource Resource { get; init; } = Resource.Invoices;
    public CrudAction Action { get; init; } = CrudAction.Create;
};

public sealed class CreateInvoiceHandler(
    IInvoiceRepository repository,
    IMapper mapper,
    IClientRepository clientRepository,
    ICurrencyExchangeService currencyExchangeService,
    ICurrencyRepository currencyRepository,
    IHttpContextAccessor httpContext,
    ICountryRepository countryRepository,
    ICustomerRepository customerRepository,
    ILogger logger,
    //i need to use this manually because it is not validating automatically
    IValidator<CurrencyExchangeModel> validator
) : ICommandHandler<CreateInvoiceCommand, InvoiceDto>
{
    public async Task<BaseResponse<InvoiceDto>> Handle(
        CreateInvoiceCommand request,
        CancellationToken cancellationToken
    )
    {
        logger.Information("Invoice starting to be created.");
        Currency currencyResp = await GetCustomerCurrency(request, cancellationToken);
        logger.Debug("customer currency retrieved @{currency}", currencyResp.CurrencyCode);
        var invoice = mapper.Map<Invoice>(request.CreateInvoiceDto);
        logger.Debug("invoice mapped @{invoice}", invoice);
        invoice.Amount = 0;
        var client = await clientRepository.GetOneAsync(
            x => x.Id == invoice.ClientId && !x.IsDeleted,
            cancellationToken
        );
        logger.Debug("client retrieved @{client}", client);
        List<ItemDtoForInvoice> items = new();
        foreach (var i in invoice.ItemsId!)
        {
            var item = client!.Items!.FirstOrDefault(x => x.Id == i)!;
            logger.Debug("item retrieved @{item}", item);
            var currency = await currencyRepository.GetOneAsync(
                x => x.Id == item.CurrencyId && !x.IsDeleted,
                cancellationToken
            );
            logger.Debug("currency retrieved of item @{currency}", currency);
            logger.Debug("currency of item @{currency.CurrencyCode}", currency.CurrencyCode);
            ItemDtoForInvoice dto = mapper.Map<ItemDtoForInvoice>(item);
            dto.Currency = currency?.CurrencyCode;

            items.Add(dto);
        }
        var invoiceDto = mapper.Map<InvoiceDto>(invoice);
        invoiceDto.Items = items;
        logger.Debug("invoiceDto mapped @{invoice}", invoiceDto);
        invoiceDto.Currency = currencyResp.CurrencyCode;
        foreach (var i in invoiceDto.Items)
        {
            //item's currencycode
            var currencyCode = i.Currency;
            if (currencyCode == "EUR")
            {
                await validator.ValidateAndThrowAsync(
                    new CurrencyExchangeModel
                    {
                        Date = invoice.IssueDate,
                        From = i.Currency,
                        To = currencyResp.CurrencyCode,
                    }
                );
                CurrencyExchangeModel modela = new()
                {
                    Date = invoice.IssueDate,
                    From = currencyCode,
                    To = currencyResp.CurrencyCode,
                    AmountFrom = i.Price,
                };
                var amounta = await currencyExchangeService.ConvertTwoCurrencies(modela, cancellationToken);
                invoice.Amount += amounta;
                continue;
            }
            await validator.ValidateAndThrowAsync(
                new CurrencyExchangeModel
                {
                    Date = invoice.IssueDate,
                    From = currencyCode,
                    To = currencyResp.CurrencyCode,
                    AmountFrom = i.Price,
                }
            );
            //converting item currency to EUR
            CurrencyExchangeModel model = new()
            {
                Date = invoice.IssueDate,
                From = currencyCode,
                To = currencyResp.CurrencyCode,
                AmountFrom = i.Price,
            };
            var amount = await currencyExchangeService.ConvertTwoCurrencies(model, cancellationToken);
            invoice.Amount += amount;
        }
        invoice.CreatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)!.Value;
        invoice.CurrencyId = currencyResp.Id;
        await repository.AddOneAsync(invoice, cancellationToken);
        var invoiceResponse = mapper.Map<InvoiceDto>(invoice);
        invoiceResponse.Items = items;
        invoiceResponse.Currency = currencyResp.CurrencyCode;
        return new SuccessResponse<InvoiceDto>(invoiceResponse, "Invoice created successfully.");
    }

    private async Task<Currency> GetCustomerCurrency(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var customerResp = await customerRepository.GetOneAsync(
            x => x.Id == request.CreateInvoiceDto.CustomerId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        var countryResp = await countryRepository.GetOneAsync(
            x => x.Id == customerResp!.CountryId && !x.IsDeleted,
            cancellationToken
        );
        var currencyResp = await currencyRepository.GetOneAsync(
            x => x.Id == countryResp!.CurrencyId && !x.IsDeleted,
            cancellationToken
        );
        return currencyResp!;
    }
}
