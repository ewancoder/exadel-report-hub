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

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public sealed record GetAllInvoicesQuery(int PageNumber = 1, int PageSize = 10) : IQuery<PaginatedListDto<InvoiceDto>>, IPermissionedRequest
{
    public List<Guid>? ClientIds => [];

    public Resource Resource => Resource.Invoices;

    public CrudAction Action => CrudAction.Read;
};

public sealed class GetAllInvoicesHandler(IInvoiceRepository repository,
    ICustomerRepository customerRepo,
    IClientRepository clientRepo,
    IACLSharedApi aCLSharedApi,
    IMapper mapper)
    : IQueryHandler<GetAllInvoicesQuery, PaginatedListDto<InvoiceDto>>
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
        var parameters = new PaginationParameters { PageNumber = request.PageNumber, PageSize = request.PageSize };
        var paginatedInvoices = await repository.GetAllPaginatedAsync(parameters, cancellationToken);

        var availableClients = await aCLSharedApi.GetUserClientsAsync(cancellationToken);
        var invoiceDtos = paginatedInvoices
            .Items.Select(invoice => new InvoiceDto
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
                Amount = invoice.Amount,
                Items = invoice.Items?.Select(i => mapper.Map<ItemDtoForClient>(i)).ToList(),
            })
            .ToList();
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
}
