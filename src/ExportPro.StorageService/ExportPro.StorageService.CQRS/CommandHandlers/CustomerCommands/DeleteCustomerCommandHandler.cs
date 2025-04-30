using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public record DeleteCustomerCommand(Guid Id) : ICommand<bool>;

public class DeleteCustomerCommandHandler(ICustomerRepository repository) : ICommandHandler<DeleteCustomerCommand, bool>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id.ToObjectId(), cancellationToken);
        if (customer == null || customer.IsDeleted)
        {
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Customer not found."],
            };
        }

        await _repository.SoftDeleteAsync(request.Id.ToObjectId(), cancellationToken);
        return new SuccessResponse<bool>(true, "Successfully deleted customer.");
    }
}
