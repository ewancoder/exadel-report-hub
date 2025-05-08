using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Refit;
using Refit;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests.Steps.InvoiceSteps;

[Binding]
public class CreateInvoiceSteps
{
    private readonly IMongoDbContext<Customer> _mongoDbContext = new MongoDbContext<Customer>();
    private ICountryApi _countryApi;
    private ICurrencyApi _currencyApi;
    private ICustomerApi _customerApi;
    private IInvoiceApi _invoiceApi;
    private CreateInvoiceDto _invoiceDto;
    //
    // [Given("The user has a valid token for creating invoice")]
    // public async Task GivenTheUserHasValidTokenForCreatingInvoice()
    // {
    //     string jwtToken = await UserLogin.Login("OwnerUserTest@gmail.com", "OwnerUserTest2@");
    //     HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:1500") };
    //     httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
    //     _countryApi = RestService.For<ICountryApi>(httpClient);
    //     _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    //     _customerApi = RestService.For<ICustomerApi>(httpClient);
    // }
}
