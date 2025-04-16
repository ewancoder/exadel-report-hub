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
            .Errors.GroupBy(x => x.PropertyName.Substring(whereShouldBeRemoved(x.PropertyName)))
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());
        TModel = default;
    }
    private int whereShouldBeRemoved(string propName)
    {
        int ind = 0;
        foreach(char i in propName)
        {
            if (i == '.')
            {
                break;
            }
             ind++;
        }
        if(ind >= propName.Length) return 0;
        return ind+1;
    }
    public T TModel { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}
