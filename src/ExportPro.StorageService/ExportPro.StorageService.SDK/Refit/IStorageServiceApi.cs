namespace ExportPro.StorageService.SDK.Refit;

public interface IStorageServiceApi
    : IClientController,
        ICountryController,
        ICurrencyController,
        ICustomerController,
        IInvoiceController;
