using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Profiles.CustomerMaps;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public class CreateCustomerCommand : ICommand<CustomerDto>
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string CountryId { get; set; }
}
public class CreateCustomerCommandHandler(ICustomerRepository repository, 
    IValidator<CreateCustomerCommand> validator) : ICommandHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repository = repository;
    private readonly IValidator<CreateCustomerCommand> _validator = validator;
    public async Task<BaseResponse<CustomerDto>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var validate = await _validator.ValidateAsync(request, cancellationToken);
        if (!validate.IsValid)
        {
            return new BaseResponse<CustomerDto>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = validate.Errors.Select(x => x.ErrorMessage).ToList(),
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
        return new BaseResponse<CustomerDto> { Data = CustomerMapper.ToDto(customer) };
    }
}