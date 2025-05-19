using ExportPro.StorageService.SDK.DTOs;
using FluentValidation;

namespace ExportPro.StorageService.API.Validations.Items;

public class ItemDtoForClientValidator : AbstractValidator<ItemDtoForClient>
{
    public ItemDtoForClientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters");
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .LessThan(double.MaxValue)
            .WithMessage("Price Is too large ");
        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithMessage("CurrencyId must not be empty")
            .Must(x => x != Guid.Empty)
            .WithMessage("CurrencyId must not be empty")
            .Must(x => x.ToString().Length == 36)
            .WithMessage("CurrencyId must be a valid Guid");
    }
}
