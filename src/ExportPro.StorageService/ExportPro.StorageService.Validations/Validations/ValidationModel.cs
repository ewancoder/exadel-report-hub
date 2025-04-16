namespace ExportPro.StorageService.Validations.Validations;

public class ValidationModel<T>
{
    public T Response { get; set; } = default!;
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}
