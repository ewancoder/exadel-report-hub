using AutoMapper;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Common.Shared.Refit;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public sealed record GetAllInvoicesQuery(int PageNumber = 1, int PageSize = 10)
    : IQuery<PaginatedListDto<InvoiceDto>>,
        IPermissionedRequest
{
    public List<Guid>? ClientIds => [];
    public Resource Resource => Resource.Invoices;
    public CrudAction Action => CrudAction.Read;
}

public sealed class GetAllInvoicesHandler(
    IInvoiceRepository repository,
    ICustomerRepository customerRepository,
    ICountryRepository countryRepository,
    IACLSharedApi aCLSharedApi,
    IClientRepository clientRepository,
    ICurrencyRepository currencyRepository,
    IMapper mapper
) : IQueryHandler<GetAllInvoicesQuery, PaginatedListDto<InvoiceDto>>
{
    public async Task<BaseResponse<PaginatedListDto<InvoiceDto>>> Handle(
        GetAllInvoicesQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.PageNumber < 1)
            return new BadRequestResponse<PaginatedListDto<InvoiceDto>>("Page number must be greater than zero.");
        if (request.PageSize < 1)
            return new BadRequestResponse<PaginatedListDto<InvoiceDto>>("Page size must be greater than zero.");

        var availableClientGuids = await aCLSharedApi.GetUserClientsAsync(cancellationToken);
        var availableClientObjectIds = availableClientGuids.Data?.Select(g => g.ToObjectId()).ToHashSet();

        var parameters = new PaginationParameters { PageNumber = request.PageNumber, PageSize = request.PageSize };

        var paginatedInvoices = await repository.GetAllPaginatedAsync(parameters, cancellationToken);
        var filteredInvoices = paginatedInvoices
            .Items.Where(i => availableClientObjectIds.Contains(i.ClientId))
            .ToList();
        var invoiceDtos = (
            await Task.WhenAll(
                filteredInvoices.Select(async invoice =>
                {
                    var currency = await GetCustomerCurrency(invoice.CustomerId, cancellationToken);
                    var items = new List<ItemDtoForInvoice>();
                    foreach (var i in invoice.ItemsId!)
                    {
                        var client = await clientRepository.GetOneAsync(
                            x => x.Items.Any(y => y.Id == i) && !x.IsDeleted,
                            cancellationToken
                        );
                        var item = client?.Items?.FirstOrDefault(y => y.Id == i);
                        if (item is not null)
                        {
                            var itemCurrency = await currencyRepository.GetOneAsync(
                                x => x.Id == item.CurrencyId && !x.IsDeleted,
                                cancellationToken
                            );
                            var itemDto = mapper.Map<ItemDtoForInvoice>(item);
                            itemDto.Currency = itemCurrency!.CurrencyCode;
                            items.Add(itemDto);
                        }
                    }

                    return new InvoiceDto
                    {
                        Id = invoice.Id.ToGuid(),
                        InvoiceNumber = invoice.InvoiceNumber,
                        IssueDate = invoice.IssueDate,
                        DueDate = invoice.DueDate,
                        Currency = currency,
                        CreatedBy = invoice.CreatedBy,
                        PaymentStatus = invoice.PaymentStatus,
                        BankAccountNumber = invoice.BankAccountNumber,
                        ClientId = invoice.ClientId.ToGuid(),
                        Items = items,
                        CustomerId = invoice.CustomerId.ToGuid(),
                        Amount = invoice.Amount,
                    };
                })
            )
        ).ToList();
        var paginatedDto = new PaginatedListDto<InvoiceDto>(
            invoiceDtos,
            paginatedInvoices.TotalCount,
            paginatedInvoices.PageNumber,
            paginatedInvoices.TotalPages
        );

        return new SuccessResponse<PaginatedListDto<InvoiceDto>>(
            paginatedDto,
            "The invoices were retrieved successfully."
        );
    }

    private async Task<string> GetCustomerCurrency(ObjectId customerId, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetOneAsync(x => x.Id == customerId && !x.IsDeleted, cancellationToken);

        var country = await countryRepository.GetOneAsync(
            x => x.Id == customer!.CountryId && !x.IsDeleted,
            cancellationToken
        );

        var currency = await currencyRepository.GetOneAsync(
            x => x.Id == country!.CurrencyId && !x.IsDeleted,
            cancellationToken
        );
        return currency!.CurrencyCode;
    }
}
