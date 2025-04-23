using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public record DeleteInvoiceCommand(ObjectId Id) : ICommand<bool>;
public class DeleteInvoiceHandler(IInvoiceRepository repository) : ICommandHandler<DeleteInvoiceCommand, bool>
{
    private readonly IInvoiceRepository _repository = repository;

    public async Task<BaseResponse<bool>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == ObjectId.Empty)
        {
            return new BaseResponse<bool>
            {
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
                Data = false,
                Messages = ["Invalid invoice ID."]
            };
        }

        var invoice = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice == null)
        {
            return new BaseResponse<bool>
            {
                ApiState = HttpStatusCode.NotFound,
                IsSuccess = false,
                Data = false,
                Messages = ["Invoice not found."]
            };
        }

        await _repository.SoftDeleteAsync(request.Id, cancellationToken);

        return new BaseResponse<bool>
        {
            ApiState = HttpStatusCode.OK,
            IsSuccess = true,
            Data = true,
            Messages = ["Invoice deleted successfully."]
        };
    }
}