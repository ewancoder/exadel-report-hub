using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Services;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.CurrencyConversion;

public sealed class CurrencyExchangeServiceValidator : AbstractValidator<CurrencyExchangeModel>
{
    public CurrencyExchangeServiceValidator(ICurrencyExchangeService currencyExchangeService)
    {
        RuleFor(x => x.From)
            .MustAsync(async (from, _) =>
            {

                var currenyExchangeModel = new CurrencyExchangeModel
                {
                    From = from,
                    Date = new DateTime(2024, 04, 17)
                };
                try
                {
                    var res = await currencyExchangeService.ExchangeRate(currenyExchangeModel);
                }
                catch 
                {
                    return false;
                }
                return true;
            }).WithMessage(x => $"Currency [{x.From}] is not supported by the  European Central Bank for conversion.");

    }
}   