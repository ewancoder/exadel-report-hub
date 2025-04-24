using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.CurrencyValidations;

public class UpdateCurrencyRequestValidator : AbstractValidator<UpdateCurrencyRequest>
{
    public UpdateCurrencyRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.")
            .Must(BeAValidObjectId).WithMessage("Invalid ObjectId format.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("Currency code is required.")
            .Length(2, 10).WithMessage("Currency code must be between 2 and 10 characters.");
    }

    private bool BeAValidObjectId(string id)
    {
        return ObjectId.TryParse(id, out _);
    }
}

public class UpdateCurrencyRequest
{
    public required string Id { get; set; }
    public required string CurrencyCode { get; set; }
}