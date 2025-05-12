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
[Scope(Tag = "DeleteCountry")]
public class DeleteCountrySteps
{
    private readonly IMongoDbContext<Country> _mongoDbContext = new MongoDbContext<Country>();
    private readonly IMongoDbContext<Currency> _mongoDbContextCurrency = new MongoDbContext<Currency>();
    private ICountryApi? _countryApi;
    private Guid _countryId;
    private ICurrencyApi? _currencyApi;
    private Guid _currencyId;

    [Given(@"The user is logged in with email '(.*)' and password '(.*)' and has necessary permissions")]
    public async Task GivenTheUserIsLoggedInWithEmailAndPasswordAndHasNecessaryPermissions(
        string email,
        string password
    )
    {
        var jwtToken = await UserLogin.Login(email, password);
        var httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _countryApi = RestService.For<ICountryApi>(httpClient);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    }

    [Given("The user created following currency and stored the currency id")]
    public async Task GivenTheFollowingCurrencyExists(Table table)
    {
        var currency = table.CreateInstance<CurrencyDto>();
        var currencyResponse = await _currencyApi!.Create(currency);
        var currencyResponseExists = await _mongoDbContextCurrency
            .Collection.Find(x => x.Id == currencyResponse.Data!.Id.ToObjectId())
            .FirstOrDefaultAsync();
        Assert.That(currencyResponseExists, Is.Not.Null);
        Assert.That(currencyResponseExists.Id, Is.EqualTo(currencyResponse.Data!.Id.ToObjectId()));
        _currencyId = currencyResponse.Data.Id;
    }

    [Given("The user created country and the stored the country id")]
    public async Task GivenTheCountryExists(Table table)
    {
        var country = table.CreateInstance<CreateCountryDto>();
        country.CurrencyId = _currencyId;
        var countryResponse = await _countryApi!.Create(country);
        var countryResponseExists = await _mongoDbContext
            .Collection.Find(x => x.Id == countryResponse.Data!.Id.ToObjectId())
            .FirstOrDefaultAsync();
        _countryId = countryResponse.Data!.Id;
        Assert.That(countryResponseExists, Is.Not.Null);
        Assert.That(countryResponseExists.Id, Is.EqualTo(countryResponse.Data.Id.ToObjectId()));
    }

    [When("The user sends the country delete request")]
    public async Task WhenTheUserSendsTheCountryCreationRequest()
    {
        await _countryApi!.Delete(_countryId);
    }

    [Then("The country should be deleted")]
    public async Task ThenTheCountryShouldBeDeleted()
    {
        var country = await _mongoDbContext.Collection.Find(x => x.Id == _countryId.ToObjectId()).FirstOrDefaultAsync();
        Assert.That(country, Is.Not.EqualTo(null));
        Assert.That(country.IsDeleted, Is.EqualTo(true));
    }

    [AfterScenario("@DeleteCountry")]
    public async Task CleanUp()
    {
        await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == _currencyId.ToObjectId());
        await _mongoDbContext.Collection.DeleteOneAsync(x => x.Id == _countryId.ToObjectId());
    }
}
