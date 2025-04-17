using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.CustomerCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CustomerHandlers;

public class DeleteCustomerCommandHandler(ICustomerRepository repository) : ICommandHandler<DeleteCustomerCommand, bool>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null || customer.IsDeleted)
        {
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = new() { "Customer not found." }
            };
        }

        await _repository.SoftDeleteAsync(request.Id, cancellationToken);
        return new BaseResponse<bool> { Data = true };
    }
}