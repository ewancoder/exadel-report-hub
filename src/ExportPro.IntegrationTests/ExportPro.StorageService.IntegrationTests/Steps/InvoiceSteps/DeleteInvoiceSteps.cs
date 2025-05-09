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
public class DeleteInvoiceSteps
{
    private readonly IMongoDbContext<Invoice> _mongoDbContext = new MongoDbContext<Invoice>();
    private ICountryApi _countryApi;
    private ICurrencyApi _currencyApi;
    private ICustomerApi _customerApi;
    private IInvoiceApi _invoiceApi;
    private IClientApi _clientApi;
    private CreateUpdateCustomerDto _customerDto;
    private Guid _invoiceId;

    [Given("The user has a valid token for deleting invoice")]
    public async Task GivenTheUserHasValidTokenForDeleting()
    {
        string jwtToken = await UserLogin.Login("OwnerUserTest@gmail.com", "OwnerUserTest2@");
        string jwtTokenForClient = await UserLogin.Login("SuperAdminTest@gmail.com", "SuperAdminTest2@");
        HttpClient httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        HttpClient httpClientForClient = HttpClientForRefit.GetHttpClient(jwtTokenForClient, 1500);
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

    [Given("The user has invoice id")]
    public async Task GivenTheUserHaveInvoiceId()
    {
        ClientDto clientDto = new() { Name = "DeleeteClientISInvoiceTest####", Description = "Description" };
        var clientResponse = await _clientApi.CreateClient(clientDto);
        CurrencyDto currency = new() { CurrencyCode = "GBP" };
        CurrencyDto currency2 = new() { CurrencyCode = "USD" };
        var currencyResponse = await _currencyApi.Create(currency);
        var currencyResponse2 = await _currencyApi.Create(currency2);
        CreateCountryDto country = new()
        {
            Name = "rIO",
            Code = "DE",
            CurrencyId = currencyResponse.Data.Id,
        };

        var countryResponse = await _countryApi.Create(country);
        CreateUpdateCustomerDto _customerDto = new()
        {
            CountryId = countryResponse.Data.Id,
            Name = "TESTUSER#####Delete",
            Email = "DeleteTESTUSER#####@gmail.com",
        };
        var customerResponse = await _customerApi.Create(_customerDto);
        CreateInvoiceDto invoiceDto = new()
        {
            InvoiceNumber = "123456789#####TESTDelete",
            IssueDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            CurrencyId = currencyResponse2.Data.Id,
            PaymentStatus = 0,
            ClientCurrencyId = currencyResponse2.Data.Id,
            CustomerId = customerResponse.Data.Id,
            ClientId = clientResponse.Data.Id,
            BankAccountNumber = "123456789####Delete",

            Items = new List<ItemDtoForClient>()
            {
                new ItemDtoForClient()
                {
                    Name = "ItemISmeExportPro",
                    Description = "Description",
                    Price = 100,
                    Status = 0,
                    CurrencyId = currencyResponse.Data.Id,
                },
            },
        };
        var invoiceResponse = await _invoiceApi.Create(invoiceDto);
        _invoiceId = invoiceResponse.Data.Id;
    }

    [When("The user sends the invoice delete request")]
    public async Task WhenTheUserSendsTheInvoiceDeleteRequest()
    {
        await _invoiceApi.Delete(_invoiceId);
    }

    [Then("The invoice should be deleted")]
    public async Task ThenTheInvoiceShouldBeDeleted()
    {
        var invoice = await _mongoDbContext
            .Collection.Find(x => x.InvoiceNumber == "123456789#####TESTDelete")
            .FirstOrDefaultAsync();
        Assert.That(invoice, Is.Not.EqualTo(null));
        Assert.That(invoice.BankAccountNumber, Is.EqualTo("123456789####Delete"));
        Assert.That(invoice.IsDeleted, Is.EqualTo(true));
    }
}
