using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class ClientNameValidator:AbstractValidator<string>
{
    public ClientNameValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x)
            .NotEmpty()
            .WithMessage("Name must not be empty")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters")
            .DependentRules(() =>
            {
                RuleFor(x => x)
                    .MustAsync(
                        async (Name, _) =>
                        {
                            var client = await clientRepository.ClientExists(Name);
                            return !client;
                        }
                    )
                    .WithMessage("Client with this name already exists");
            });
    }
}