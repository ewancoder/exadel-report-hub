using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public record DeleteInvoiceCommand(Guid Id) : ICommand<bool>;

public class DeleteInvoiceHandler(IInvoiceRepository repository) : ICommandHandler<DeleteInvoiceCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return new BaseResponse<bool>
            {
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
                Data = false,
                Messages = ["Invalid invoice ID."],
            };
        }

        var invoice = await repository.GetByIdAsync(request.Id.ToObjectId(), cancellationToken);
        if (invoice == null)
        {
            return new BaseResponse<bool>
            {
                ApiState = HttpStatusCode.NotFound,
                IsSuccess = false,
                Data = false,
                Messages = ["Invoice not found."],
            };
        }

        await repository.SoftDeleteAsync(request.Id.ToObjectId(), cancellationToken);

        return new BaseResponse<bool>
        {
            ApiState = HttpStatusCode.OK,
            IsSuccess = true,
            Data = true,
            Messages = ["Invoice deleted successfully."],
        };
    }
}
