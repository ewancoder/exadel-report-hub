using System.Net.Mail;
using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public sealed record CreateCustomersCommand(List<CreateUpdateCustomerDto> Customers) : ICommand<int>;

public sealed class CreateCustomersCommandHandler(
    IHttpContextAccessor httpContext,
    ICustomerRepository customerRepository,
    ICountryRepository countryRepository,
    IMapper mapper
) : ICommandHandler<CreateCustomersCommand, int>
{
    public async Task<BaseResponse<int>> Handle(
        CreateCustomersCommand request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await ValidateCustomersAsync(request, cancellationToken);

        if (validationResult is not null)
            return validationResult;

        return await CreateCustomersAsync(request, cancellationToken);
    }

    private async Task<BaseResponse<int>?> ValidateCustomersAsync(
        CreateCustomersCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request.Customers.Count == 0)
            return new BadRequestResponse<int>("No customers provided for creation.");

        var validationErrors = new Dictionary<int, List<string>>();

        for (var i = 0; i < request.Customers.Count; i++)
        {
            var customerDto = request.Customers[i];
            var errors = await ValidateSingleCustomerAsync(customerDto, cancellationToken);

            if (errors.Count > 0)
                validationErrors[i] = errors;
        }

        if (validationErrors.Count > 0)
            return CreateValidationErrorResponse(validationErrors);

        return null;
    }

    private async Task<List<string>> ValidateSingleCustomerAsync(
        CreateUpdateCustomerDto customerDto,
        CancellationToken cancellationToken
    )
    {
        var errors = new List<string>();

        ValidateRequiredFields(customerDto, errors);

        if (errors.Count > 0) return errors;

        await ValidateCountryExistsAsync(customerDto.CountryId, errors, cancellationToken);
        ValidateEmailFormat(customerDto.Email, errors);
        await ValidateNoDuplicatesAsync(customerDto, errors, cancellationToken);

        return errors;
    }

    private async Task<BaseResponse<int>> CreateCustomersAsync(
        CreateCustomersCommand request,
        CancellationToken cancellationToken
    )
    {
        var successCount = 0;

        foreach (var customerDto in request.Customers)
        {
            var customer = mapper.Map<Customer>(customerDto);
            customer.CreatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
            await customerRepository.AddOneAsync(customer, cancellationToken);
            successCount++;
        }

        return new SuccessResponse<int>(successCount, $"{successCount} customers created successfully.");
    }

    private void ValidateRequiredFields(CreateUpdateCustomerDto customerDto, List<string> errors)
    {
        if (string.IsNullOrEmpty(customerDto.Name))
            errors.Add("Name is required.");

        if (string.IsNullOrEmpty(customerDto.Email))
            errors.Add("Email is required.");

        if (string.IsNullOrEmpty(customerDto.Address))
            errors.Add("Address is required.");

        if (customerDto.CountryId == Guid.Empty)
            errors.Add("CountryId is required.");
    }

    private async Task ValidateCountryExistsAsync(
        Guid countryId,
        List<string> errors,
        CancellationToken cancellationToken
    )
    {
        var country = await countryRepository.GetOneAsync(
            x => x.Id == countryId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );

        if (country == null)
            errors.Add("CountryId does not exist.");
    }

    private void ValidateEmailFormat(string email, List<string> errors)
    {
        try
        {
            var addr = new MailAddress(email);
            if (addr.Address != email)
                errors.Add("Invalid email format.");
        }
        catch
        {
            errors.Add("Invalid email format.");
        }
    }

    private async Task ValidateNoDuplicatesAsync(
        CreateUpdateCustomerDto customerDto,
        List<string> errors,
        CancellationToken cancellationToken
    )
    {
        var existingEmail = await customerRepository.GetOneAsync(
            x => x.Email == customerDto.Email && !x.IsDeleted,
            cancellationToken
        );

        if (existingEmail != null)
            errors.Add("Email already exists.");

        var existingName = await customerRepository.GetOneAsync(
            x => x.Name == customerDto.Name && !x.IsDeleted,
            cancellationToken
        );

        if (existingName != null)
            errors.Add("Name already exists.");

        var existingAddress = await customerRepository.GetOneAsync(
            x => x.Address == customerDto.Address && !x.IsDeleted,
            cancellationToken
        );

        if (existingAddress != null)
            errors.Add("Address already exists.");
    }

    private BaseResponse<int> CreateValidationErrorResponse(Dictionary<int, List<string>> validationErrors)
    {
        var errorMessages = validationErrors
            .SelectMany(kvp => kvp.Value.Select(error => $"Customer {kvp.Key + 1}: {error}"))
            .ToList();

        return new BadRequestResponse<int> { Messages = errorMessages };
    }
}