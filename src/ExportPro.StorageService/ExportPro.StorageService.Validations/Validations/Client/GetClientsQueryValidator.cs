using ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Client;

public class GetClientsQueryValidator : AbstractValidator<GetClientsQuery>
{
    public GetClientsQueryValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.Top).GreaterThan(0).WithMessage("Top must be higher than 0");

        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be greater than or equal to 0")
            .DependentRules(() =>
            {
                RuleFor(x => x.Skip)
                    .MustAsync(
                        async (skip, cancellationToken) =>
                        {
                            var res = await clientRepository.HigherThanMaxSize(skip, cancellationToken);
                            return !res;
                        }
                    )
                    .WithMessage("Skip can't be higher than the max size");
            });
    }
}
