using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public record GetInvoiceByIdQuery(string Id) : IQuery<InvoiceDto>;

public class GetInvoiceByIdHandler(IInvoiceRepository repository, IMapper mapper)
    : IQueryHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    private readonly IInvoiceRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.Id, out ObjectId objectId))
        {
            return new BaseResponse<InvoiceDto>
            {
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
                Messages = ["Invalid invoice ID format."],
            };
        }

        var invoice = await _repository.GetByIdAsync(objectId, cancellationToken);
        if (invoice == null)
        {
            return new BaseResponse<InvoiceDto>
            {
                ApiState = HttpStatusCode.NotFound,
                IsSuccess = false,
                Messages = ["Invoice not found."],
            };
        }

        var dto = new InvoiceDto
        {
            Id = invoice.Id.ToString(),
            InvoiceNumber = invoice.InvoiceNumber,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            CurrencyId = invoice.CurrencyId,
            CustomerId = invoice.CustomerId,
            PaymentStatus = invoice.PaymentStatus,
            BankAccountNumber = invoice.BankAccountNumber,
            ClientId = invoice.ClientId,
            Items = invoice.Items.Select(x => _mapper.Map<ItemDtoForClient>(x)).ToList(),
        };

        return new BaseResponse<InvoiceDto>
        {
            ApiState = HttpStatusCode.OK,
            IsSuccess = true,
            Data = dto,
        };
    }
}
