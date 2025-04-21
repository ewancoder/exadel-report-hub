using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.CustomerCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.CQRS.Handlers.CustomerHandlers;

public class CreateCustomerCommandHandler(ICustomerRepository repository
    
    , IMapper mapper,IValidator<CreateCustomerCommand> validator)
    : ICommandHandler<CreateCustomerCommand, CustomerResponse>
{
    private readonly IMapper _mapper = mapper;
    private readonly ICustomerRepository _repository = repository;
    private readonly IValidator<CreateCustomerCommand> _validator = validator;
    public async Task<BaseResponse<CustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var validate = await _validator.ValidateAsync(request, cancellationToken);
            if (!validate.IsValid)
            {
                return new BaseResponse<CustomerResponse>
                {
                    IsSuccess = false,
                    ApiState = HttpStatusCode.BadRequest,
                    Messages = validate.Errors.Select(x=>x.ErrorMessage).ToList(),
                };
            }
        var countryIdString = request.CountryId.ToString();

        var customer = new Customer
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Email = request.Email,
            CountryId = countryIdString,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            IsDeleted = false,
        };
        await _repository.AddOneAsync(customer, cancellationToken);
        return new BaseResponse<CustomerResponse> { Data = _mapper.Map<CustomerResponse>(customer) };
    }
}
