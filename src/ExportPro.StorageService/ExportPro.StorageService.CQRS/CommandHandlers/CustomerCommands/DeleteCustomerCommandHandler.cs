using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public sealed record DeleteCustomerCommand(Guid Id) : ICommand<bool>;

public sealed class DeleteCustomerCommandHandler(ICustomerRepository repository)
    : ICommandHandler<DeleteCustomerCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (customer == null || customer.IsDeleted)
            return new NotFoundResponse<bool>("Customer not found.");
        await repository.SoftDeleteAsync(request.Id.ToObjectId(), cancellationToken);
        return new SuccessResponse<bool>(true, "Successfully deleted customer.");
    }
}
