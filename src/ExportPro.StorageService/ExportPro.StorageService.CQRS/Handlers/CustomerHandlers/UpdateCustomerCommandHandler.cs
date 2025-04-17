using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.CustomerCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CustomerHandlers;

public class UpdateCustomerCommandHandler(ICustomerRepository repository, IMapper mapper)
    : ICommandHandler<UpdateCustomerCommand, Customer>
{
    private readonly ICustomerRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<Customer>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existingCustomer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existingCustomer is null || existingCustomer.IsDeleted)
        {
            return new BaseResponse<Customer>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = new() { "Customer not found." }
            };
        }

        _mapper.Map(request, existingCustomer); // updates only relevant fields (Name, Email, CountryId, UpdatedAt)

        await _repository.UpdateOneAsync(existingCustomer, cancellationToken);

        return new BaseResponse<Customer> { Data = existingCustomer };
    }
}