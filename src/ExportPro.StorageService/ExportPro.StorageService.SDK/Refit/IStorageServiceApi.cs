namespace ExportPro.StorageService.SDK.Refit;

public interface IStorageServiceApi
{
    IClientController Client { get; }
    ICurrencyController Currency { get; }
    IInvoiceController Invoice { get; }
    ICountryController Country { get; }
    ICustomerController Customer { get; }
}
