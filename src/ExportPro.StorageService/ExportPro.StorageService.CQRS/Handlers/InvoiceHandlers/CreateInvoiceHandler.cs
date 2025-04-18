﻿using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.CQRS.Commands.InvoiceCommands;
using MongoDB.Bson;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.Handlers.InvoiceHandlers;

public class CreateInvoiceHandler(IInvoiceRepository repository) : ICommandHandler<CreateInvoiceCommand, Invoice>
{
    private readonly IInvoiceRepository _repository = repository;

    public async Task<BaseResponse<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        // Validate required fields
        var validationErrors = ValidateCreateInvoiceCommand(request);
        if (validationErrors.Any())
        {
            return new BaseResponse<Invoice>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = validationErrors
            };
        }

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
            ClientId = request.ClientId,
            ItemIds = request.ItemIds ?? new List<string>()
        };

        await _repository.AddOneAsync(invoice, cancellationToken);

        return new BaseResponse<Invoice>
        {
            Data = invoice,
            ApiState = HttpStatusCode.Created,
            IsSuccess = true,
            Messages = new List<string> { "Invoice created successfully." }
        };
    }

    private List<string> ValidateCreateInvoiceCommand(CreateInvoiceCommand request)
    {
        var errors = new List<string>();

        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.InvoiceNumber))
            errors.Add("Invoice number is required.");

        if (request.Amount <= 0)
            errors.Add("Amount must be greater than zero.");

        if (request.DueDate < request.IssueDate)
            errors.Add("Due date cannot be earlier than issue date.");

        // Validate ObjectId formats
        if (!string.IsNullOrWhiteSpace(request.CurrencyId) && !ObjectId.TryParse(request.CurrencyId, out _))
            errors.Add("Invalid currency ID format.");

        if (!string.IsNullOrWhiteSpace(request.ClientId) && !ObjectId.TryParse(request.ClientId, out _))
            errors.Add("Invalid client ID format.");

        if (request.ItemIds != null && request.ItemIds.Any(id => !string.IsNullOrWhiteSpace(id) && !ObjectId.TryParse(id, out _)))
            errors.Add("One or more item IDs are in an invalid format.");

        return errors;
    }
}