using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public sealed class UpdateInvoiceCommand : ICommand<Invoice>
{
    public required string Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public double Amount { get; set; }
    public string? CurrencyId { get; set; }
    public Status? PaymentStatus { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? ClientId { get; set; }
    public List<string>? ItemIds { get; set; }
}
public sealed class UpdateInvoiceHandler(IInvoiceRepository repository) : ICommandHandler<UpdateInvoiceCommand, Invoice>
{
    private readonly IInvoiceRepository _repository = repository;

    public async Task<BaseResponse<Invoice>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var validationErrors = ValidateUpdateInvoiceCommand(request);
        if (!ObjectId.TryParse(request.Id, out ObjectId objectId))
        {
            return new BaseResponse<Invoice>
            {
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
                Messages = ["Invalid invoice ID format."]
            };
        }

        var existing = await _repository.GetByIdAsync(objectId, cancellationToken);
        
        if (existing == null)
        {
            return new BaseResponse<Invoice>
            {
                ApiState = HttpStatusCode.NotFound,
                IsSuccess = false,
                Messages = ["Invoice not found."]
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
        //existing.ItemIds = request.ItemIds ?? new List<string>();

        await _repository.UpdateOneAsync(existing, cancellationToken);

        return new BaseResponse<Invoice>
        {
            Data = existing,
            ApiState = HttpStatusCode.OK,
            IsSuccess = true,
            Messages = ["Invoice updated successfully."]
        };
    }

    private List<string> ValidateUpdateInvoiceCommand(UpdateInvoiceCommand request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Id) || !ObjectId.TryParse(request.Id, out _))
            errors.Add("Invoice ID is required and must be a valid ObjectId.");

        if (string.IsNullOrWhiteSpace(request.InvoiceNumber))
            errors.Add("Invoice number is required.");

        if (request.Amount <= 0)
            errors.Add("Amount must be greater than zero.");

        if (request.DueDate < request.IssueDate)
            errors.Add("Due date cannot be earlier than issue date.");

        if (!string.IsNullOrWhiteSpace(request.CurrencyId) && !ObjectId.TryParse(request.CurrencyId, out _))
            errors.Add("Invalid currency ID format.");

        if (!string.IsNullOrWhiteSpace(request.ClientId) && !ObjectId.TryParse(request.ClientId, out _))
            errors.Add("Invalid client ID format.");

        if (request.ItemIds != null && request.ItemIds.Any(id => !string.IsNullOrWhiteSpace(id) && !ObjectId.TryParse(id, out _)))
            errors.Add("One or more item IDs are in an invalid format.");

        return errors;
    }
}