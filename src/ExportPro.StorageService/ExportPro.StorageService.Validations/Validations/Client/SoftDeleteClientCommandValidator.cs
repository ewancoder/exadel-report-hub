using ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class SoftDeleteClientCommandValidator : AbstractValidator<SoftDeleteClientCommand>
{
    public SoftDeleteClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .Must(id =>
            {
                return ObjectId.TryParse(id, out _);
            })
            .WithMessage("The Client Id is not valid in format.")
            .DependentRules(() =>
            {
                RuleFor(x => x.ClientId)
                    .MustAsync(
                        async (id, cancellationtoken) =>
                        {
                            var client = await clientRepository.GetOneAsync(
                                x => x.Id == ObjectId.Parse(id) && !x.IsDeleted,
                                cancellationtoken
                            );
                            return client != null;
                        }
                    )
                    .WithMessage("The Client Id does not exist");
            });
    }
}
