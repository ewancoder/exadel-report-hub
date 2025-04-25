using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportPro.StorageService.CQRS.QueryHandlers.PlanQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public class GetPlanValidator:AbstractValidator<GetPlanQuery>
{
    public GetPlanValidator(IClientRepository clientRepository)
    {

        RuleFor(x => x.PlanId)
                .NotEmpty()
                .WithMessage("Plan Id  cannot be empty.")
                .Must(id =>
                {
                    return ObjectId.TryParse(id, out _);
                })
                .WithMessage("The Plan Id is not valid in format.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.PlanId)
                    .MustAsync(
                async (id, cancellationToken) =>
                {
                var plan = await clientRepository.GetPlan(id, cancellationToken);
                return plan != null;
            }
                    )
                    .WithMessage("The Plan Id does not exist");
                });
    }
}
