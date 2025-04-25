using ExportPro.StorageService.CQRS.QueryHandlers.CurrencyQueries;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.CurrencyValidations;

public class GetCurrencyByIdQueryValidator : AbstractValidator<GetCurrencyByIdQuery>
{
    public GetCurrencyByIdQueryValidator()
    {
        RuleFor(x => x.Id.ToString()).IsValidObjectId();
    }
}