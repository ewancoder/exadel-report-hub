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
        if (request.Customers.Count == 0) return new BadRequestResponse<int>("No customers provided for creation.");

        // Dictionary to collect validation errors by customer index
        var validationErrors = new Dictionary<int, List<string>>();

        // Validate each customer before processing any
        for (var i = 0; i < request.Customers.Count; i++)
        {
            var customerDto = request.Customers[i];
            List<string> errors = [];

            // Validate required fields
            if (string.IsNullOrEmpty(customerDto.Name))
                errors.Add("Name is required.");

            if (string.IsNullOrEmpty(customerDto.Email))
                errors.Add("Email is required.");

            if (string.IsNullOrEmpty(customerDto.Address))
                errors.Add("Address is required.");

            if (customerDto.CountryId == Guid.Empty)
                errors.Add("CountryId is required.");

            // Skip further validation if basic fields are missing
            if (errors.Count > 0)
            {
                validationErrors[i] = errors;
                continue;
            }

            // Validate country exists
            var country = await countryRepository.GetOneAsync(
                x => x.Id == customerDto.CountryId.ToObjectId() && !x.IsDeleted,
                cancellationToken
            );

            if (country == null) errors.Add("CountryId does not exist.");

            // Validate email format
            if (!IsValidEmail(customerDto.Email)) errors.Add("Invalid email format.");

            // Validate no duplicates in database
            var existingEmail = await customerRepository.GetOneAsync(
                x => x.Email == customerDto.Email && !x.IsDeleted,
                cancellationToken
            );

            if (existingEmail != null) errors.Add("Email already exists.");

            var existingName = await customerRepository.GetOneAsync(
                x => x.Name == customerDto.Name && !x.IsDeleted,
                cancellationToken
            );

            if (existingName != null) errors.Add("Name already exists.");

            // If there are validation errors, add them to the dictionary
            if (errors.Count > 0) validationErrors[i] = errors;

            // Validate Address exists
            var existingAddress = await customerRepository.GetOneAsync(
                x => x.Address == customerDto.Address && !x.IsDeleted,
                cancellationToken
            );

            if (existingAddress != null) errors.Add("Address already exists.");
        }

        // Return validation errors if any
        if (validationErrors.Count > 0)
        {
            // Convert validation errors to a flat list of messages
            var errorMessages = validationErrors
                .SelectMany(kvp => kvp.Value.Select(error => $"Customer {kvp.Key + 1}: {error}"))
                .ToList();

            return new BadRequestResponse<int>
            {
                Messages = errorMessages
            };
        }

        // All customers passed validation, proceed with creation
        var successCount = 0;
        foreach (var customerDto in request.Customers)
        {
            var customer = mapper.Map<Customer>(customerDto);
            customer.CreatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)!.Value;
            await customerRepository.AddOneAsync(customer, cancellationToken);
            successCount++;
        }

        return new SuccessResponse<int>(successCount, $"{successCount} customers created successfully.");
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}