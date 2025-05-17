using ExportPro.StorageService.SDK.DTOs;
using FluentValidation;

namespace ExportPro.StorageService.API.Validations.Items;

public class ListItemsValidator : AbstractValidator<List<ItemDtoForClient>>
{
    public ListItemsValidator()
    {
        RuleForEach(y => y)
            .ChildRules(x =>
            {
                x.RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("Name must not be empty for item {CollectionIndex}")
                    .MinimumLength(3)
                    .WithMessage("Name must be at least 3 characters long for item {CollectionIndex}")
                    .MaximumLength(50)
                    .WithMessage("Name must not exceed 50 characters for item {CollectionIndex}");
                x.RuleFor(x => x.Price)
                    .GreaterThan(0)
                    .WithMessage("Price must be greater than 0 for item {CollectionIndex}")
                    .LessThan(double.MaxValue)
                    .WithMessage("Price Is too large  for item {CollectionIndex}");
                x.RuleFor(x => x.CurrencyId)
                    .NotEmpty()
                    .WithMessage("CurrencyId must not be empty for item {CollectionIndex}")
                    .Must(x => x != Guid.Empty)
                    .WithMessage("CurrencyId must not be empty for item {CollectionIndex}")
                    .Must(x => x.ToString().Length == 36)
                    .WithMessage("CurrencyId must be a valid Guid for item {CollectionIndex}");
            });
    }
}
