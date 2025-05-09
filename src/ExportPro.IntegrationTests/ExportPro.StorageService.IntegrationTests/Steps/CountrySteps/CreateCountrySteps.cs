using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ExportPro.StorageService.IntegrationTests.Steps.CountrySteps;

[Binding]
public class CreateCountrySteps
{
    private readonly IMongoDbContext<Country> _mongoDbContext = new MongoDbContext<Country>();
    private readonly IMongoDbContext<Currency> _mongoDbContextCurrency = new MongoDbContext<Currency>();
    private ICountryApi _countryApi;
    private ICurrencyApi _currencyApi;
    private Guid _currencyId;
    private CreateCountryDto _createCountryDto;

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
    }

    [Given("The user created following currency and stored the currency id")]
    public async Task GivenTheFollowingCurrencyExists(Table table)
    {
        CurrencyDto cur = table.CreateInstance<CurrencyDto>();
        var currency = await _currencyApi.Create(cur);
        var currencyExists = await _mongoDbContextCurrency
            .Collection.Find(x =>
                x.CurrencyCode == currency.Data.CurrencyCode && x.CreatedBy == currency.Data.CreatedBy
            )
            .FirstOrDefaultAsync();
        _currencyId = currency.Data.Id;
        Assert.That(currencyExists, Is.Not.EqualTo(null));
        Assert.That(currencyExists.CurrencyCode, Is.EqualTo(cur.CurrencyCode));
    }

    [Given("The user wants to create following country")]
    public void GivenTheUserHasACountryToCreate(Table table)
    {
        _createCountryDto = table.CreateInstance<CreateCountryDto>();
        _createCountryDto.CurrencyId = _currencyId;
    }

    [When("The user sends the country creation request")]
    public async Task WhenTheUserSendsTheCountryCreationRequest()
    {
        await _countryApi.Create(_createCountryDto);
    }

    [Then("The country should be saved in the database")]
    public async Task ThenTheCountryShouldBeSavedInTheDb()
    {
        var country = await _mongoDbContext.Collection.Find(x => x.Name == "TestUsa####").FirstOrDefaultAsync();
        Assert.That(country, Is.Not.EqualTo(null));
        Assert.That(country.Code, Is.EqualTo(("TESTCOUNTRYCODE")));
    }
}
