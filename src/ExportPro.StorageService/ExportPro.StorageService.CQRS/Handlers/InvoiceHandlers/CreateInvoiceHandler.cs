using System.Net;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.CQRS.Commands.InvoiceCommands;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Handlers.InvoiceHandlers;

public class CreateInvoiceHandler(IRepository<Invoice> repository) : ICommandHandler<CreateInvoiceCommand, Invoice>
{
    private readonly IRepository<Invoice> _repository = repository;

    public async Task<BaseResponse<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = new Invoice
        {
            Id = ObjectId.GenerateNewId(),
            InvoiceNumber = request.InvoiceNumber,
            IssueDate = request.IssueDate,
            DueDate = request.DueDate,
            Amount = request.Amount,
            CurrencyId = request.CurrencyId,
            PaymentStatus = request.PaymentStatus,
            BankAccountNumber = request.BankAccountNumber,
            // ClientId = request.ClientId,
            // ItemIds = request.ItemIds ?? new List<string>()
        };

        await _repository.AddOneAsync(invoice, cancellationToken);
        return new BaseResponse<Invoice>
        {
            Data = invoice,
            ApiState = HttpStatusCode.Created,
            Messages = new List<string> { "Invoice created successfully." }
        };
    }
}