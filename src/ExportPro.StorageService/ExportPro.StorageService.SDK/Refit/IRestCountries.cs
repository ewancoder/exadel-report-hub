using ExportPro.StorageService.SDK.Responses;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface IRestCountries
{
    [Get("/all")]
    Task<List<RestCountries>> GetAllCountries();
}
