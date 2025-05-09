using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests.Steps.InvoiceSteps;

[Binding]
public class CreateInvoiceSteps
{
    private readonly IMongoDbContext<Invoice> _mongoDbContext = new MongoDbContext<Invoice>();
    private ICountryApi _countryApi;
    private ICurrencyApi _currencyApi;
    private ICustomerApi _customerApi;
    private IInvoiceApi _invoiceApi;
    private IClientApi _clientApi;
    private CreateInvoiceDto _invoiceDto;

    [Given("The user has a valid token for creating invoice")]
    public async Task GivenTheUserHasValidTokenForCreatingInvoice()
    {
        string jwtToken = await UserLogin.Login("OwnerUserTest@gmail.com", "OwnerUserTest2@");
        string jwtTokenForClient = await UserLogin.Login("SuperAdminTest@gmail.com", "SuperAdminTest2@");
        HttpClient httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        var httpClientForClient = HttpClientForRefit.GetHttpClient(jwtTokenForClient, 1500);
        _countryApi = RestService.For<ICountryApi>(httpClient);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
        _invoiceApi = RestService.For<IInvoiceApi>(
            httpClient,
            new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    }
                ),
            }
        );
        _customerApi = RestService.For<ICustomerApi>(httpClient);
        _clientApi = RestService.For<IClientApi>(httpClientForClient);
    }

    [Given("The user has a invoice to create")]
    public async Task GivenTheUserHasInvoiceToCreate()
    {
        ClientDto clientDto = new() { Name = "ClientISInvoiceTest####", Description = "Description" };
        var clientResponse = await _clientApi.CreateClient(clientDto);
        CurrencyDto currency = new() { CurrencyCode = "GBP" };
        CurrencyDto currency2 = new() { CurrencyCode = "USD" };
        var currencyResponse = await _currencyApi.Create(currency);
        var currencyResponse2 = await _currencyApi.Create(currency2);
        CreateCountryDto country = new()
        {
            Name = "Germany",
            Code = "DE",
            CurrencyId = currencyResponse.Data.Id,
        };

        var countryResponse = await _countryApi.Create(country);
        CreateUpdateCustomerDto _customerDto = new()
        {
            CountryId = countryResponse.Data.Id,
            Name = "TESTUSER#####",
            Email = "TESTUSER#####@gmail.com",
        };
        var customerResponse = await _customerApi.Create(_customerDto);
        _invoiceDto = new()
        {
            InvoiceNumber = "123456789#####TEST",
            IssueDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            CurrencyId = currencyResponse2.Data.Id,
            PaymentStatus = 0,
            ClientCurrencyId = currencyResponse2.Data.Id,
            CustomerId = customerResponse.Data.Id,
            ClientId = clientResponse.Data.Id,
            BankAccountNumber = "123456789",

            Items = new List<ItemDtoForClient>()
            {
                new ItemDtoForClient()
                {
                    Name = "ItemISme",
                    Description = "Description",
                    Price = 100,
                    Status = 0,
                    CurrencyId = currencyResponse.Data.Id,
                },
            },
        };
    }

    [When("the user sends the invoice creation request")]
    public async Task WhenTheUserSendsTheInvoiceCreationRequest()
    {
        await _invoiceApi.Create(_invoiceDto);
    }

    [Then("the invoice should be saved in the database")]
    public async Task ThenTheInvoiceShouldBeSavedInTheDb()
    {
        var invoice = await _mongoDbContext
            .Collection.Find(x => x.InvoiceNumber == "123456789#####TEST")
            .FirstOrDefaultAsync();
        Assert.That(invoice, Is.Not.EqualTo(null));
        Assert.That(invoice.BankAccountNumber, Is.EqualTo("123456789"));
    }
}
