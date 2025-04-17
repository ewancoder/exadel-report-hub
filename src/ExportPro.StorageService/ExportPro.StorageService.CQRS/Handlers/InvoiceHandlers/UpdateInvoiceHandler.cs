using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.InvoiceCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Handlers.InvoiceHandlers;

public class UpdateInvoiceHandler(IInvoiceRepository repository) : ICommandHandler<UpdateInvoiceCommand, Invoice>
{
    private readonly IInvoiceRepository _repository = repository;

    public async Task<BaseResponse<Invoice>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing == null)
        {
            return new BaseResponse<Invoice>
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
        existing.CurrencyId = request.CurrencyId;
        existing.PaymentStatus = request.PaymentStatus;
        existing.BankAccountNumber = request.BankAccountNumber;
        existing.ClientId = request.ClientId;
        existing.ItemIds = request.ItemIds ?? new List<string>();

        await _repository.UpdateOneAsync(existing, cancellationToken);

        return new BaseResponse<Invoice>
        {
            Data = existing,
            ApiState = HttpStatusCode.OK,
            Messages = new List<string> { "Invoice updated successfully." }
        };
    }
}