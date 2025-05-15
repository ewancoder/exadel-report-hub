using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Api.Validations.Client;

public sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.Client.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters")
            .DependentRules(() =>
            {
                RuleFor(x => x.Client.Name)
                    .MustAsync(
                        async (name, cancellationToken) =>
                        {
                            var client = await clientRepository.GetOneAsync(
                                x => x.Name == name && !x.IsDeleted,
                                cancellationToken
                            );
                            return client == null;
                        }
                    )
                    .WithMessage("Client with this name already exists");
            });
    }
}
