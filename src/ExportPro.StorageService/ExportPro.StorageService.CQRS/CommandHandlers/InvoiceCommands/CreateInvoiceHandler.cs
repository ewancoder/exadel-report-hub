using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using ExportPro.StorageService.SDK.Services;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;
public class CreateInvoiceCommand : ICommand<InvoiceResponse>
{
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public string? CurrencyId { get; set; }
    public Status? PaymentStatus { get; set; }
    public string? CustomerId { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? ClientId { get; set; }
    public List<ItemDtoForClient>? Items { get; set; }
}
public class CreateInvoiceHandler(
    IInvoiceRepository repository,
    IMapper mapper,
    ICurrencyExchangeService currencyExchangeService,
    ICurrencyRepository currencyRepository,
    ICustomerRepository customerRepository,
    ICountryRepository countryRepository,
    IValidator<CurrenyExchangeModel> validator,
    IValidator<CreateInvoiceCommand> validatorInvoice
) : ICommandHandler<CreateInvoiceCommand, InvoiceResponse>
{
    private readonly IInvoiceRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrencyExchangeService _currencyExchangeService = currencyExchangeService;
    private readonly ICurrencyRepository _currencyRepository = currencyRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly ICountryRepository _countryRepository = countryRepository;
    private readonly IValidator<CurrenyExchangeModel> _validator = validator;
    private readonly IValidator<CreateInvoiceCommand> _validatorInvoice = validatorInvoice;
    public async Task<BaseResponse<InvoiceResponse>> Handle(
        CreateInvoiceCommand request,
        CancellationToken cancellationToken
    )
    {
        var validateInvoice = await _validatorInvoice.ValidateAsync(request);

        if (!validateInvoice.IsValid)
        {
            return new BaseResponse<InvoiceResponse>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = validateInvoice.Errors.Select(x => x.ErrorMessage).ToList(),
            };
        }

        var invoice = new Invoice
        {
            Id = ObjectId.GenerateNewId(),
            InvoiceNumber = request.InvoiceNumber,
            IssueDate = request.IssueDate,
            DueDate = request.DueDate,
            CurrencyId = request.CurrencyId,
            PaymentStatus = request.PaymentStatus,
            BankAccountNumber = request.BankAccountNumber,
            ClientId = request.ClientId,
            CustomerId = request.CustomerId,
            Items = request.Items.Select(_ => _mapper.Map<Item>(_)).ToList(),
        };

        foreach (var i in invoice.Items)
        {
            i.Id = ObjectId.GenerateNewId();
        }
        var customer = await _customerRepository.GetOneAsync(
            x => x.Id.ToString() == invoice.CustomerId,
            CancellationToken.None
        );
        var country = await _countryRepository.GetOneAsync(
            x => x.Id.ToString() == customer.CountryId,
            CancellationToken.None
        );

        var customer_currency = await _currencyRepository.GetCurrencyCodeById(country.CurrencyId);
        CurrenyExchangeModel cur = new()
        {
            From = customer_currency.CurrencyCode,
            Date = new DateTime(2024, 4, 17)
        };

        var to_euro_echange_rate = 1.0;
        var validat = _validator.ValidateAsync(cur);
        if (!validat.Result.IsValid)
        {
            return new BaseResponse<InvoiceResponse>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = validat.Result.Errors.Select(x => x.ErrorMessage).ToList(),
            };
        }
        if (customer_currency.CurrencyCode != "EUR")
        {
            var exchangeRate = await _currencyExchangeService.ExchangeRate(cur);
            to_euro_echange_rate = exchangeRate;
        }
        invoice.Amount = 0;
        foreach (var i in invoice.Items)
        {
            var currency = await _currencyRepository.GetCurrencyCodeById(i.CurrencyId);
            if (currency == null)
            {
                return new BaseResponse<InvoiceResponse>
                {
                    ApiState = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Messages = ["Currency not found."]
                };
            }
            string currencyCode = currency.CurrencyCode;
            CurrenyExchangeModel currencyExchangeModel = new()
            {
                From = currencyCode,
                Date = invoice.IssueDate,
            };
            var validateCurrency = await _validator.ValidateAsync(currencyExchangeModel);
            if (!validateCurrency.IsValid)
            {
                return new BaseResponse<InvoiceResponse>
                {
                    IsSuccess = false,
                    ApiState = HttpStatusCode.BadRequest,
                    Messages = validateCurrency.Errors.Select(x => x.ErrorMessage).ToList(),
                };
            }
            var exchangeRate = 1.0;
            if (currencyCode != "EUR")
            {

                exchangeRate = await _currencyExchangeService.ExchangeRate(currencyExchangeModel);
            }

            var exc = to_euro_echange_rate / exchangeRate;
            i.Price = i.Price * exc;
            invoice.Amount += i.Price;
        }
        await _repository.AddOneAsync(invoice, cancellationToken);

        return new BaseResponse<InvoiceResponse>
        {
            Data = _mapper.Map<InvoiceResponse>(invoice),
            ApiState = HttpStatusCode.Created,
            IsSuccess = true,
            Messages = ["Invoice created successfully."],
        };
    }
}