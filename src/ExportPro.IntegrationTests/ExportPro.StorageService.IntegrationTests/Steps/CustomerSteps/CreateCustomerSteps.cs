using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests.Steps.CustomerSteps;

[Binding]
public class CreateCustomerSteps
{
    private readonly IMongoDbContext<Customer> _mongoDbContext = new MongoDbContext<Customer>();
    private ICountryApi _countryApi;
    private ICurrencyApi _currencyApi;
    private ICustomerApi _customerApi;
    private CreateUpdateCustomerDto _customerDto;

    [Given("The user has a valid token for creating customer")]
    public async Task GivenTheUserHasValidTokenForCreatingCustomer()
    {
        string jwtToken = await UserLogin.Login("OwnerUserTest@gmail.com", "OwnerUserTest2@");
        HttpClient httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _countryApi = RestService.For<ICountryApi>(httpClient);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
        _customerApi = RestService.For<ICustomerApi>(httpClient);
    }

    [Given("The user has a customer to create")]
    public async Task GivenTheUserHaveCustomerToCreate()
    {
        CurrencyDto cur = new() { CurrencyCode = "PPP" };
        var currency = await _currencyApi.Create(cur);
        CreateCountryDto countryDto = new()
        {
            Code = "USS",
            Name = "TestUsaCUSTOMER####",
            CurrencyId = currency.Data.Id,
        };
        var country = await _countryApi.Create(countryDto);
        _customerDto = new()
        {
            CountryId = country.Data.Id,
            Name = "TESTUSER####",
            Email = "TESTUSER####@gmail.com",
        };
    }

    [When("the user sends the customer creation request")]
    public async Task WhenTheUserSendsTheCustomerCreationRequest()
    {
        await _customerApi.Create(_customerDto);
    }

    [Then("the customer should be saved in the database")]
    public async Task ThenTheCountryShouldBeSavedInTheDb()
    {
        var country = await _mongoDbContext
            .Collection.Find(x => x.Email == "TESTUSER####@gmail.com")
            .FirstOrDefaultAsync();
        Assert.That(country, Is.Not.EqualTo(null));
        Assert.That(country.Name, Is.EqualTo(("TESTUSER####")));
    }
}
