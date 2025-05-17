using ExportPro.StorageService.SDK.Refit;
using Refit;

public class StorageServiceApi : IStorageServiceApi
{
    public IClientController Client { get; }
    public ICurrencyController Currency { get; }
    public IInvoiceController Invoice { get; }
    public ICountryController Country { get; }
    public ICustomerController Customer { get; }

    public StorageServiceApi(HttpClient http, RefitSettings? settings = null)
    {
        Client = RestService.For<IClientController>(http, settings);
        Currency = RestService.For<ICurrencyController>(http, settings);
        Invoice = RestService.For<IInvoiceController>(http, settings);
        Country = RestService.For<ICountryController>(http, settings);
        Customer = RestService.For<ICustomerController>(http, settings);
    }
}
