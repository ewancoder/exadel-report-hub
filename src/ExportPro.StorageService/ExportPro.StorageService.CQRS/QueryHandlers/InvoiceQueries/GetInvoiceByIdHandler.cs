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

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public sealed record GetInvoiceByIdQuery(Guid Id) : IQuery<InvoiceDto>, IPermissionedRequest
{
    public List<Guid>? ClientIds => null;
    public Resource Resource => Resource.Invoices;
    public CrudAction Action => CrudAction.Read;
};

public sealed class GetInvoiceByIdHandler(IInvoiceRepository repository,
    ICustomerRepository customerRepo,
    IClientRepository clientRepository, 
    ICurrencyRepository currencyRepo, 
    IACLSharedApi aclApi,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper)
    : IQueryHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    public async Task<BaseResponse<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
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
                Action = request.Action
            }
        );

        if (!permission)
            return new NotFoundResponse<InvoiceDto>("You do not have permission to view this invoice.");

        var customer = await customerRepo.GetByIdAsync(
            invoice.CustomerId,
            cancellationToken
        );

        var currency = await currencyRepo.GetCurrencyCodeById(invoice.CurrencyId);

        var itemDtos = await Task.WhenAll(invoice.Items.Select(async item =>
        {
            var itemDto = mapper.Map<ItemDtoForClient>(item);
            var currency = await currencyRepo.GetCurrencyCodeById(item.CurrencyId);
            itemDto.CurrencyName = currency?.CurrencyCode ?? string.Empty;
            return itemDto;
        }));
        var dto = new InvoiceDto
        {
            Id = invoice.Id.ToGuid(),
            InvoiceNumber = invoice.InvoiceNumber,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            CurrencyId = invoice.CurrencyId.ToGuid(),
            CustomerId = invoice.CustomerId.ToGuid(),
            PaymentStatus = invoice.PaymentStatus,
            BankAccountNumber = invoice.BankAccountNumber,
            ClientId = invoice.ClientId.ToGuid(),
            ClientCurrencyId = invoice.ClientCurrencyId.ToGuid(),
            Amount = invoice.Amount,
            Items = [.. itemDtos],
            CustomerName = customer?.Name,
            ClientName = client?.Name,
            ClientCurrencyName = currency?.CurrencyCode,

        };
        return new SuccessResponse<InvoiceDto>(dto, "Invoice found successfully.");
    }
}
