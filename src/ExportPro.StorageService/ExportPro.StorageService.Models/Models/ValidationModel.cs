using FluentValidation.Results;

namespace ExportPro.StorageService.Models.Models;

public class ValidationModel<T>
{
    public ValidationModel()
    {
        TModel = default;
        ValidationErrors = null;
    }

    public ValidationModel(T Request)
    {
        TModel = Request;
        ValidationErrors = null;
    }

    public ValidationModel(ValidationResult validationResult)
    {
        ValidationErrors = validationResult
            .Errors.GroupBy(x => x.PropertyName.Replace("Clientdto.", ""))
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());
        TModel = default;
    }

    public T TModel { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}
