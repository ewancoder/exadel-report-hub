using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.Invoice;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.Invoice;

public class UpdateInvoiceHandler(IRepository<Models.Models.Invoice> repository) : ICommandHandler<UpdateInvoiceCommand, Models.Models.Invoice>
{
    private readonly IRepository<Models.Models.Invoice> _repository = repository;

    public async Task<BaseResponse<Models.Models.Invoice>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing == null)
        {
            return new BaseResponse<Models.Models.Invoice>
            {
                ApiState = HttpStatusCode.NotFound,
                IsSuccess = false,
                Messages = new List<string> { "Invoice not found." }
            };
        }

        existing.InvoiceNumber = request.InvoiceNumber;
        existing.IssueDate = request.IssueDate;
        existing.DueDate = request.DueDate;
        existing.Amount = request.Amount;
        existing.Currency = request.Currency;
        existing.PaymentStatus = request.PaymentStatus;
        existing.BankAccountNumber = request.BankAccountNumber;
        existing.ClientId = request.ClientId;
        existing.ItemIds = request.ItemIds ?? new List<string>();

        await _repository.UpdateOneAsync(existing, cancellationToken);

        return new BaseResponse<Models.Models.Invoice>
        {
            Data = existing,
            ApiState = HttpStatusCode.OK,
            Messages = new List<string> { "Invoice updated successfully." }
        };
    }
}