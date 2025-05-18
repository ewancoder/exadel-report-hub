using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Services;
using FluentValidation;

namespace ExportPro.StorageService.Api.Validations.CurrencyConversion;

public sealed class CurrencyExchangeServiceValidator : AbstractValidator<CurrencyExchangeModel>
{
    public CurrencyExchangeServiceValidator(ICurrencyExchangeService currencyExchangeService)
    {
        RuleFor(x => x.To)
            .MustAsync(
                async (to, CancellationToken) =>
                {
                    var currenyExchangeModel = new CurrencyExchangeModel
                    {
                        From = to,

                        Date = new DateTime(2024, 04, 17),
                    };
                    try
                    {
                        await currencyExchangeService.ExchangeRate(currenyExchangeModel, CancellationToken);
                    }
                    catch
                    {
                        return false;
                    }

                    return true;
                }
            )
            .WithMessage(x => $"Currency [{x.To}] is not supported by the  European Central Bank for conversion.");
        RuleFor(x => x.From)
            .MustAsync(
                async (from, CancellationToken) =>
                {
                    var currenyExchangeModel = new CurrencyExchangeModel
                    {
                        From = from,

                        Date = new DateTime(2024, 04, 17),
                    };
                    try
                    {
                        await currencyExchangeService.ExchangeRate(currenyExchangeModel, CancellationToken);
                    }
                    catch
                    {
                        return false;
                    }

                    return true;
                }
            )
            .WithMessage(x => $"Currency [{x.From}] is not supported by the  European Central Bank for conversion.");
    }
}
