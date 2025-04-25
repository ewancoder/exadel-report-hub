using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations;

public static class CustomValidators
{
    public static IRuleBuilderOptions<T, string> IsValidObjectId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("'{PropertyName}' must be a valid ObjectId format.");
    }
}