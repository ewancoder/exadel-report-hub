using AutoMapper;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Common.Shared.Refit;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Refit;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Serilog;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public sealed record GetInvoiceByIdQuery(Guid Id) : IQuery<InvoiceDto>, IPermissionedRequest
{
    public List<Guid>? ClientIds => null;
    public Resource Resource => Resource.Invoices;
    public CrudAction Action => CrudAction.Read;
}

public sealed class GetInvoiceByIdHandler(
    IInvoiceRepository repository,
    ICountryRepository countryRepository,
    ICurrencyRepository currencyRepository,
    IClientRepository clientRepository,
    ICustomerRepository customerRepository,
    IACLSharedApi aclApi,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    ILogger logger
) : IQueryHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    public async Task<BaseResponse<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        logger.Information("Invoice found: {@Invoice}", invoice);
        if (invoice == null)
            return new NotFoundResponse<InvoiceDto>("Invoice not found.");
        var client = await clientRepository.GetOneAsync(
            x => x.Id == invoice.ClientId && !x.IsDeleted,
            cancellationToken
        );
        var permission = await aclApi.CheckPermissionAsync(
            new Common.Shared.Models.CheckPermissionRequest
            {
                ClientId = client?.Id.ToGuid(),
                UserId = TokenHelper.GetUserId(httpContextAccessor.HttpContext?.User).ToGuid(),
                Resource = request.Resource,
                Action = request.Action,
            }
        );

        if (!permission.Data)
            return new NotFoundResponse<InvoiceDto>("You do not have permission to view this invoice.");
        var customerCurrency = await GetCustomerCurrency(invoice.CustomerId, cancellationToken);
        logger.Debug("Customer Currency: {@customerCurrency}", customerCurrency);
        List<ItemDtoForInvoice> items = new();
        int cnt = 0;
        foreach (var item in invoice.ItemsId)
        {
            var itemClient = await clientRepository.GetOneAsync(
                x => x.Id == invoice.ClientId && !x.IsDeleted,
                cancellationToken
            );
            logger.Debug("Item Client: {@itemClient} cnt: {cnt}", itemClient, cnt++);
            var itemResp = itemClient!.Items.FirstOrDefault(x => x.Id == item);
            logger.Debug("Item: {@itemResp }", itemResp);
            var itemCurrency = await currencyRepository.GetOneAsync(
                x => x.Id == itemResp.CurrencyId && !x.IsDeleted,
                cancellationToken
            );
            var itemDto = mapper.Map<ItemDtoForInvoice>(itemResp);
            logger.Debug("Item Currency: {@itemCurrency}", itemCurrency);
            itemDto.Currency = itemCurrency!.CurrencyCode;
            items.Add(itemDto);
        }
        var dto = new InvoiceDto
        {
            Id = invoice.Id.ToGuid(),
            InvoiceNumber = invoice.InvoiceNumber,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Currency = customerCurrency,
            Items = items,
            PaymentStatus = invoice.PaymentStatus,
            CreatedBy = invoice.CreatedBy,
            BankAccountNumber = invoice.BankAccountNumber,
            ClientId = invoice.ClientId.ToGuid(),
            CustomerId = invoice.CustomerId.ToGuid(),
            Amount = invoice.Amount,
        };
        return new SuccessResponse<InvoiceDto>(dto, "Invoice found successfully.");
    }

    private async Task<string> GetCustomerCurrency(ObjectId customerId, CancellationToken cancellationToken)
    {
        logger.Debug("GetCustomerCurrency");
        var customer = await customerRepository.GetOneAsync(x => x.Id == customerId && !x.IsDeleted, cancellationToken);
        logger.Debug("Customer: {@customer}", customer);
        var country = await countryRepository.GetOneAsync(
            x => x.Id == customer!.CountryId && !x.IsDeleted,
            cancellationToken
        );
        logger.Debug("Country: {@country}", country);
        var currency = await currencyRepository.GetOneAsync(
            x => x.Id == country!.CurrencyId && !x.IsDeleted,
            cancellationToken
        );
        logger.Debug("Currency: {@currency}", currency);
        return currency!.CurrencyCode;
    }
}
