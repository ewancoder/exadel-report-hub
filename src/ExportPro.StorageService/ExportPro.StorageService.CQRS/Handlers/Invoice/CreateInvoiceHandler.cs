using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.Invoice;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.Invoice;

public class CreateInvoiceHandler(IRepository<Models.Models.Invoice> repository) : ICommandHandler<CreateInvoiceCommand, Models.Models.Invoice>
{
    private readonly IRepository<Models.Models.Invoice> _repository = repository;

    public async Task<BaseResponse<Models.Models.Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = new Models.Models.Invoice
        {
            Id = ObjectId.GenerateNewId(),
            InvoiceNumber = request.InvoiceNumber,
            IssueDate = request.IssueDate,
            DueDate = request.DueDate,
            Amount = request.Amount,
            Currency = request.Currency,
            PaymentStatus = request.PaymentStatus,
            BankAccountNumber = request.BankAccountNumber,
            ClientId = request.ClientId,
            ItemIds = request.ItemIds ?? new List<string>()
        };

        await _repository.AddOneAsync(invoice, cancellationToken);
        return new BaseResponse<Models.Models.Invoice>
        {
            Data = invoice,
            ApiState = HttpStatusCode.Created,
            Messages = new List<string> { "Invoice created successfully." }
        };
    }
}

