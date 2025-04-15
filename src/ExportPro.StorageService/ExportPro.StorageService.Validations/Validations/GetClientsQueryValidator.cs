using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations;

public class GetClientsQueryValidator : AbstractValidator<GetClientsQuery>
{
    public GetClientsQueryValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.top)
            .NotEmpty()
            .WithMessage("Top must be not empty")
            .GreaterThan(0)
            .WithMessage("Top must be higher than 0");

        RuleFor(x => x.skip)
            .NotEmpty()
            .WithMessage("Skip must be not empty")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be greater than or equal to 0")
            .MustAsync(
                async (skip, _) =>
                {
                    var res = await clientRepository.HigherThanMaxSize(skip);
                    return res;
                }
            );
    }
}
