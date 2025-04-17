using ExportPro.StorageService.SDK.DTOs;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Plans;

public class PlansValidator : AbstractValidator<PlansDto>
{
    public PlansValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date cannot be empty")
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("The start date must be greater than or equal to Today");
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date cannot be empty")
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("The end date must be greater than or equal to Today");
        RuleFor(x => x)
            .Must(x =>
            {
                var res = x.StartDate.CompareTo(x.EndDate);
                if (res >= 0)
                    return false;
                return true;
            })
            .WithMessage("The end date must be greater than start date");
    }
}
