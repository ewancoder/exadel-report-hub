using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Client;

public class GetClientsByIdQueryValidator : AbstractValidator<GetClientByIdQuery>
{
    public GetClientsByIdQueryValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .Must(id =>
            {
                return ObjectId.TryParse(id, out _);
            })
            .WithMessage("The Client Id is not valid in format.")
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustAsync(
                        async (id, _) =>
                        {
                            var client = await clientRepository.GetClientById(id);
                            return client != null;
                        }
                    )
                    .WithMessage("The Client Id does not exist");
            });
    }
}
