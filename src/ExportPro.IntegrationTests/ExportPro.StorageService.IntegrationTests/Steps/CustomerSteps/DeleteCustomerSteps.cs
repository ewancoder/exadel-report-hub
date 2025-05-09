using ExportPro.Common.Shared.Extensions;
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
[Scope(Tag = "DeleteCustomer")]
public class DeleteCustomerSteps
{
    private readonly IMongoDbContext<Customer> _mongoDbContext = new MongoDbContext<Customer>();
    private readonly IMongoDbContext<Country> _mongoDbContextCountry = new MongoDbContext<Country>();
    private readonly IMongoDbContext<Currency> _mongoDbContextCurrency = new MongoDbContext<Currency>();
    private ICountryApi _countryApi;
    private ICurrencyApi _currencyApi;
    private ICustomerApi _customerApi;
    private Guid _customerId;
    private Guid _countryId;
    private Guid _currencyId;

    [Given("The user is logged in with email and password and has necessary permissions")]
    public async Task GivenTheUserIsLoggedInWithEmailAndPasswordAndHasNecessaryPermissions(Table table)
    {
        string jwtToken = await UserLogin.Login(table.Rows[0]["Email"], table.Rows[0]["Password"]);
        HttpClient httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _countryApi = RestService.For<ICountryApi>(httpClient);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
        _customerApi = RestService.For<ICustomerApi>(httpClient);
    }

    [Given("The user has created following currency and stored the currency id")]
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

    [Given("The user has created following country and stored the country id")]
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

    [Given("The user has created following customer and stored the customer id")]
    public async Task GivenTheUserHasCreatedFollowingCustomerAndStoredTheCustomerId(Table table)
    {
        CreateUpdateCustomerDto customerDto = table.CreateInstance<CreateUpdateCustomerDto>();
        customerDto.CountryId = _countryId;
        var customer = await _customerApi.Create(customerDto);
        var customerExists = await _mongoDbContext
            .Collection.Find(x => x.Name == customer.Data.Name)
            .FirstOrDefaultAsync();
        Assert.That(customerExists, Is.Not.EqualTo(null));
        Assert.That(customerExists.Name, Is.EqualTo(customer.Data.Name));
        _customerId = customer.Data.Id;
    }

    [When("The user sends the customer delete request")]
    public async Task WhenTheUserSendsTheCustomerCreationRequest()
    {
        await _customerApi.Delete(_customerId);
    }

    [Then("The customer should be deleted")]
    public async Task ThenTheCustomerShouldBeDeleted()
    {
        var customer = await _mongoDbContext
            .Collection.Find(x => x.Id == _customerId.ToObjectId())
            .FirstOrDefaultAsync();
        Assert.That(customer, Is.Not.EqualTo(null));
        Assert.That(customer.Name, Is.EqualTo(("TESTUSER####TESTCUSTOMER")));
        Assert.That(customer.IsDeleted, Is.EqualTo(true));
    }
}
