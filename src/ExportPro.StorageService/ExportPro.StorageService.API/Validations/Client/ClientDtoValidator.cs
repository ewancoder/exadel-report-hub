using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using FluentValidation;

namespace ExportPro.StorageService.Api.Validations.Client;

public sealed class ClientDtoValidator : AbstractValidator<ClientDto>
{
    public ClientDtoValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters");
    }
}
