using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Client;

public class GetClientsQueryValidator : AbstractValidator<GetClientsQuery>
{
    public GetClientsQueryValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.top)
            .GreaterThan(0)
            .WithMessage("Top must be higher than 0");

        RuleFor(x => x.skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be greater than or equal to 0")
            .DependentRules(() =>
            {
                RuleFor(x => x.skip)
                    .MustAsync(
                        async (skip, _) =>
                        {
                            var res = await clientRepository.HigherThanMaxSize(skip);
                            return !res;
                        }
                    )
                    .WithMessage("Skip can't be higher than the max size");
            });
    }
}
