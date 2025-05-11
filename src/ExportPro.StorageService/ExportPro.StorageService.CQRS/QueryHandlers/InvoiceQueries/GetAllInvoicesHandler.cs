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
using ExportPro.StorageService.SDK.PaginationParams;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public sealed record GetAllInvoicesQuery(int PageNumber = 1, int PageSize = 10) : IQuery<PaginatedListDto<InvoiceDto>>, IPermissionedRequest
{
    public List<Guid>? ClientIds => [];

    public Resource Resource => Resource.Invoices;

    public CrudAction Action => CrudAction.Read;
};

public sealed class GetAllInvoicesHandler(
    IInvoiceRepository repository,
    ICustomerRepository customerRepo,
    IClientRepository clientRepo,
    IACLSharedApi aCLSharedApi,
    ICurrencyRepository currencyRepo,
    IMapper mapper)
    : IQueryHandler<GetAllInvoicesQuery, PaginatedListDto<InvoiceDto>>
{
    public async Task<BaseResponse<PaginatedListDto<InvoiceDto>>> Handle(
        GetAllInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1)
            return new BadRequestResponse<PaginatedListDto<InvoiceDto>>("Page number must be greater than zero.");
        if (request.PageSize < 1)
            return new BadRequestResponse<PaginatedListDto<InvoiceDto>>("Page size must be greater than zero.");

        var availableClientGuids = await aCLSharedApi.GetUserClientsAsync(cancellationToken);
        var availableClientObjectIds = availableClientGuids.Select(g => g.ToObjectId()).ToHashSet();

        var parameters = new PaginationParameters
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var paginatedInvoices = await repository.GetAllPaginatedAsync(parameters, cancellationToken);

        var filteredInvoices = paginatedInvoices.Items
            .Where(i => availableClientObjectIds.Contains(i.ClientId))
            .ToList();

        var invoiceDtos = await Task.WhenAll(filteredInvoices.Select(async invoice =>
        {
            var client = await clientRepo.GetOneAsync(x => x.Id == invoice.ClientId, cancellationToken);
            var clientCurrency = await currencyRepo.GetCurrencyCodeById(invoice.ClientCurrencyId);
            var customer = await customerRepo.GetByIdAsync(invoice.CustomerId, cancellationToken);

            var itemDtos = await Task.WhenAll(invoice.Items.Select(async item =>
            {
                var dto = mapper.Map<ItemDtoForClient>(item);
                var itemCurrency = await currencyRepo.GetCurrencyCodeById(item.CurrencyId);
                dto.CurrencyName = itemCurrency?.CurrencyCode ?? string.Empty;
                return dto;
            }));

            return new InvoiceDto
            {
                Id = invoice.Id.ToGuid(),
                InvoiceNumber = invoice.InvoiceNumber,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                CurrencyId = invoice.CurrencyId.ToGuid(),
                PaymentStatus = invoice.PaymentStatus,
                BankAccountNumber = invoice.BankAccountNumber,
                ClientId = invoice.ClientId.ToGuid(),
                ClientCurrencyId = invoice.ClientCurrencyId.ToGuid(),
                ClientCurrencyName = clientCurrency?.CurrencyCode,
                ClientName = client?.Name,
                CustomerName = customer?.Name,
                Amount = invoice.Amount,
                Items = [.. itemDtos]
            };
        }));

        var paginatedDto = new PaginatedListDto<InvoiceDto>(
            [.. invoiceDtos],
            filteredInvoices.Count,
            parameters.PageNumber,
            (int)Math.Ceiling((double)filteredInvoices.Count / parameters.PageSize)
        );

        return new SuccessResponse<PaginatedListDto<InvoiceDto>>(
            paginatedDto,
            "The invoices were retrieved successfully."
        );
    }
}
