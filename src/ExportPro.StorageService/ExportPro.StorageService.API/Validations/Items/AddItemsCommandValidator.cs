using ExportPro.Common.Shared.Extensions;
using ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.API.Validations.Items;

public class AddItemsCommandValidator : AbstractValidator<CreateItemCommand>
{
    public AddItemsCommandValidator(ICurrencyRepository currencyRepository)
    {
        RuleFor(x => x.CurrencyId)
            .MustAsync(
                async (currencyId, cancellationToken) =>
                {
                    var currency = await currencyRepository.GetOneAsync(
                        x => x.Id == currencyId.ToObjectId() && !x.IsDeleted,
                        cancellationToken
                    );
                    return currency != null;
                }
            )
            .WithMessage("CurrencyId does not exist");
    }
}
