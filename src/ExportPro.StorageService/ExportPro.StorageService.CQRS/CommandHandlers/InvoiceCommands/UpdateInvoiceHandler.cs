using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public sealed class UpdateInvoiceCommand : ICommand<Invoice>
{
    public Guid Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public double Amount { get; set; }
    public Guid CurrencyId { get; set; }
    public Status? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }
    public Guid ClientId { get; set; }
    public List<Guid>? ItemIds { get; set; }
}

public sealed class UpdateInvoiceHandler(IInvoiceRepository repository) : ICommandHandler<UpdateInvoiceCommand, Invoice>
{
    public async Task<BaseResponse<Invoice>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (existing == null)
            return new NotFoundResponse<Invoice>("Invoice not found.");
        existing.InvoiceNumber = request.InvoiceNumber;
        existing.IssueDate = request.IssueDate;
        existing.DueDate = request.DueDate;
        existing.Amount = request.Amount;
        existing.CurrencyId = request.CurrencyId.ToObjectId();
        existing.PaymentStatus = request.PaymentStatus;
        existing.BankAccountNumber = request.BankAccountNumber;
        existing.ClientId = request.ClientId.ToObjectId();
        await repository.UpdateOneAsync(existing, cancellationToken);
        return new SuccessResponse<Invoice>(existing, "Invoice updated successfully.");
    }
}
