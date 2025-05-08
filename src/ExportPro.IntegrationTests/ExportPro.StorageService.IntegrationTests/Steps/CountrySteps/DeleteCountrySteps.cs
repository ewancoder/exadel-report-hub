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
public class DeleteCountrySteps
{
    private readonly IMongoDbContext<Country> _mongoDbContext = new MongoDbContext<Country>();
    private ICurrencyApi _currencyApi;
    private ICountryApi _countryApi;
    private Guid _countryId;

    [Given("The user has a valid token for deleting a country")]
    public async Task GivenTheUserHasValidTokenForDeleting()
    {
        string jwtToken = await UserLogin.Login("OwnerUserTest@gmail.com", "OwnerUserTest2@");
        HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:1500") };
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
        _countryApi = RestService.For<ICountryApi>(httpClient);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    }

    [Given("The user has country id")]
    public async Task GivenTheUserHasCountryId()
    {
        CurrencyDto cur = new() { CurrencyCode = "QQQ" };
        var currency = await _currencyApi.Create(cur);
        CreateCountryDto createCountryDto = new()
        {
            Code = "USA",
            Name = "ShouldBeDeletedTest####",
            CurrencyId = currency.Data.Id,
        };
        var country = await _countryApi.Create(createCountryDto);
        _countryId = country.Data.Id;
    }

    [When("The user sends the country delete request")]
    public async Task WhenTheUserSendsTheCountryCreationRequest()
    {
        await _countryApi.Delete(_countryId);
    }

    [Then("The country should be deleted")]
    public async Task ThenTheCountryShouldBeSavedInTheDb()
    {
        var country = await _mongoDbContext
            .Collection.Find(x => x.Name == "ShouldBeDeletedTest####")
            .FirstOrDefaultAsync();
        Assert.That(country, Is.Not.EqualTo(null));
        Assert.That(country.IsDeleted, Is.EqualTo((true)));
    }
}
