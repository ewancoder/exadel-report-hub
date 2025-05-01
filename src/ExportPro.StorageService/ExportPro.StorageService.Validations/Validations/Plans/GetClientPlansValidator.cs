using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.CQRS.QueryHandlers.PlanQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public class GetClientPlansValidator : AbstractValidator<GetClientPlansQuery>
{
    public GetClientPlansValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("The client id is required")
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .DependentRules(() =>
            {
                RuleFor(x => x.ClientId)
                    .MustAsync(
                        async (clientid, cancellationToken) =>
                        {
                            var client = await clientRepository.GetOneAsync(
                                x => x.Id == clientid.ToObjectId() && !x.IsDeleted,
                                cancellationToken
                            );
                            return client != null;
                        }
                    )
                    .WithMessage("The Client id does not exist in the client");
            })
            .DependentRules(() =>
            {
                RuleFor(x => x.Top).GreaterThan(0).WithMessage("Top must be higher than 0");
                RuleFor(x => x.Skip)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Skip must be greater than or equal to 0")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x)
                            .MustAsync(
                                async (x, cancellationToken) =>
                                {
                                    var res = await clientRepository.GetOneAsync(
                                        y => y.Id == x.ClientId.ToObjectId() && !y.IsDeleted,
                                        cancellationToken
                                    );
                                    var cnt = res.Plans?.Count;
                                    return x.Skip <= cnt;
                                }
                            )
                            .WithMessage("Skip can't be higher than the max size");
                    });
            });
    }
}
