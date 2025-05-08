using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests.Steps.CountrySteps;

[Binding]
public class CreateCountrySteps
{
    private readonly IMongoDbContext<Country> _mongoDbContext = new MongoDbContext<Country>();
    private ICountryApi _countryApi;
    private ICurrencyApi _currencyApi;
    private CreateCountryDto _createCountryDto;

    [Given("The user has a valid token for creating a country")]
    public async Task GivenTheUserHasValidTokenForCreating()
    {
        string jwtToken = await UserLogin.Login("OwnerUserTest@gmail.com", "OwnerUserTest2@");
        HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:1500") };
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
        _countryApi = RestService.For<ICountryApi>(httpClient);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    }

    [Given("The user has a country")]
    public async Task GivenTheUserHasCountry()
    {
        CurrencyDto cur = new() { CurrencyCode = "QQQ" };
        var currency = await _currencyApi.Create(cur);
        _createCountryDto = new()
        {
            Code = "USA",
            Name = "TestUsa####",
            CurrencyId = currency.Data.Id,
        };
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
        Assert.That(country.Code, Is.EqualTo(("USA")));
    }
}
