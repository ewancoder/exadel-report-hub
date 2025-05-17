using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Api.Validations.Client;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.ClientDto.Name)
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
    }
}
