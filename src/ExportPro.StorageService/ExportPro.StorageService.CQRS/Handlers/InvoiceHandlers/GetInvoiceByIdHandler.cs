using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.InvoiceQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Handlers.InvoiceHandlers;

public class GetInvoiceByIdHandler(IInvoiceRepository repository)
    : IQueryHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    private readonly IInvoiceRepository _repository = repository;

    public async Task<BaseResponse<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.Id, out ObjectId objectId))
        {
            return new BaseResponse<InvoiceDto>
            {
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
                Messages = new List<string> { "Invalid invoice ID format." }
            };
        }

        var invoice = await _repository.GetByIdAsync(objectId, cancellationToken);
        if (invoice == null)
        {
            return new BaseResponse<InvoiceDto>
            {
                ApiState = HttpStatusCode.NotFound,
                IsSuccess = false,
                Messages = new List<string> { "Invoice not found." }
            };
        }

        var dto = new InvoiceDto
        {
            Id = invoice.Id.ToString(),
            InvoiceNumber = invoice.InvoiceNumber,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Amount = invoice.Amount,
            CurrencyId = invoice.CurrencyId,
            PaymentStatus = invoice.PaymentStatus,
            BankAccountNumber = invoice.BankAccountNumber,
            ClientId = invoice.ClientId,
            ItemIds = invoice.ItemIds
        };

        return new BaseResponse<InvoiceDto>
        {
            ApiState = HttpStatusCode.OK,
            IsSuccess = true,
            Data = dto
        };
    }
}