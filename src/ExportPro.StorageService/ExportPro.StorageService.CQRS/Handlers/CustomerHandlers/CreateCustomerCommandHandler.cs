using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.CustomerCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.CQRS.Handlers.CustomerHandlers;

public class CreateCustomerCommandHandler(ICustomerRepository repository, IMapper mapper)
    : ICommandHandler<CreateCustomerCommand, CustomerResponse>
{
    private readonly IMapper _mapper = mapper;
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<CustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        // Validate CountryId format if provided
        string countryIdString = null;
        if (!string.IsNullOrEmpty(request.CountryId))
        {
            if (!ObjectId.TryParse(request.CountryId, out var parsedCountryId))
            {
                return new BaseResponse<CustomerResponse>
                {
                    IsSuccess = false,
                    ApiState = HttpStatusCode.BadRequest,
                    Messages = new() { "Invalid CountryId format. Must be a valid MongoDB ObjectId." },
                };
            }
            countryIdString = parsedCountryId.ToString();
        }

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
