using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.CustomerCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Handlers.CustomerHandlers;

public class CreateCustomerCommandHandler(ICustomerRepository repository, IMapper mapper)
    : ICommandHandler<CreateCustomerCommand, Customer>
{
    private readonly ICustomerRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<Customer>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = _mapper.Map<Customer>(request);

        await _repository.AddOneAsync(customer, cancellationToken);

        return new BaseResponse<Customer> { Data = customer };
    }
}