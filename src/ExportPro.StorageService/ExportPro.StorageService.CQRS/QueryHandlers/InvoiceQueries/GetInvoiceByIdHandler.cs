using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public sealed record GetInvoiceByIdQuery(Guid Id) : IQuery<InvoiceDto>;

public sealed class GetInvoiceByIdHandler(IInvoiceRepository repository, IMapper mapper)
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
        var dto = new InvoiceDto
        {
            Id = invoice.Id.ToGuid(),
            InvoiceNumber = invoice.InvoiceNumber,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            CurrencyId = invoice.CurrencyId.ToGuid(),
            PaymentStatus = invoice.PaymentStatus,
            BankAccountNumber = invoice.BankAccountNumber,
            ClientId = invoice.ClientId.ToGuid(),
            CustomerId = invoice.CustomerId.ToGuid(),
            Amount = invoice.Amount,
            Items = invoice.Items?.Select(x => mapper.Map<ItemDtoForClient>(x)).ToList(),
        };
        return new SuccessResponse<InvoiceDto>(dto, "Invoice found successfully.");
    }
}
