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
using TechTalk.SpecFlow.Assist;

namespace ExportPro.StorageService.IntegrationTests.Steps.CustomerSteps;

[Binding]
[Scope(Tag = "CreateCustomer")]
public class CreateCustomerSteps
{
    private readonly IMongoDbContext<Customer> _mongoDbContext = new MongoDbContext<Customer>();
    private readonly IMongoDbContext<Country> _mongoDbContextCountry = new MongoDbContext<Country>();
    private readonly IMongoDbContext<Currency> _mongoDbContextCurrency = new MongoDbContext<Currency>();
    private ICountryApi _countryApi;
    private ICurrencyApi _currencyApi;
    private ICustomerApi _customerApi;
    private Guid _countryId;
    private Guid _currencyId;
    private CreateUpdateCustomerDto _customerDto;

    [Given(@"The user is logged in with email '(.*)' and password '(.*)' and has necessary permissions")]
    public async Task GivenTheUserIsLoggedInWithEmailAndPasswordAndHasNecessaryPermissions(
        string email,
        string password
    )
    {
        string jwtToken = await UserLogin.Login(email, password);
        HttpClient httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _countryApi = RestService.For<ICountryApi>(httpClient);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
        _customerApi = RestService.For<ICustomerApi>(httpClient);
    }

    [Given("The user created following currency and stored the currency id")]
    public async Task GivenTheUserCreatedFollowingCurrencyAndStoredTheCurrencyId(Table table)
    {
        CurrencyDto cur = table.CreateInstance<CurrencyDto>();
        var currency = await _currencyApi.Create(cur);
        var currencyExists = await _mongoDbContextCurrency
            .Collection.Find(x =>
                x.CurrencyCode == currency.Data.CurrencyCode && x.CreatedBy == currency.Data.CreatedBy
            )
            .FirstOrDefaultAsync();
        Assert.That(currencyExists, Is.Not.EqualTo(null));
        Assert.That(currencyExists.CurrencyCode, Is.EqualTo(cur.CurrencyCode));
        _currencyId = currency.Data.Id;
    }

    [Given("The user created following country and stored the country id")]
    public async Task GivenTheUserCreatedFollowingCountryAndStoredTheCountryId(Table table)
    {
        CreateCountryDto countryDto = table.CreateInstance<CreateCountryDto>();
        countryDto.CurrencyId = _currencyId;
        var country = await _countryApi.Create(countryDto);
        var countryExists = await _mongoDbContextCountry
            .Collection.Find(x => x.Name == country.Data.Name)
            .FirstOrDefaultAsync();
        Assert.That(countryExists, Is.Not.EqualTo(null));
        Assert.That(countryExists.Name, Is.EqualTo(country.Data.Name));
        _countryId = country.Data.Id;
    }

    [Given("The user wants to create following customer")]
    public async Task GivenTheUserWantsToCreateFollowingCustomer(Table table)
    {
        _customerDto = table.CreateInstance<CreateUpdateCustomerDto>();
        _customerDto.CountryId = _countryId;
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
            .Collection.Find(x => x.Email == "TESTUSER####TESTCUSTOMER@gmail.com")
            .FirstOrDefaultAsync();
        Assert.That(country, Is.Not.EqualTo(null));
        Assert.That(country.Name, Is.EqualTo(("TESTUSER####TESTCUSTOMER")));
    }
}
