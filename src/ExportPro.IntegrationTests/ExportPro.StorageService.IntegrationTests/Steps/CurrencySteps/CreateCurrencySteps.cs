using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using NSubstitute.ReturnsExtensions;
using Refit;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests.Steps.CurrencySteps;

[Binding]
public class CreateCurrencySteps
{
    private readonly IMongoDbContext<Currency> _mongoDbContext = new MongoDbContext<Currency>();
    private CurrencyDto _currency;
    private ICurrencyApi _currencyApi;

    [Given("The user has a valid token for creating")]
    public async Task GivenTheUserHasValidToken()
    {
        string jwtToken = await UserLogin.Login("OwnerUserTest@gmail.com", "OwnerUserTest2@");
        HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:1500") };
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    }

    [Given(@"The user has a currency")]
    public void GivenTheUserHaveCurrency()
    {
        _currency = new() { CurrencyCode = "DDD" };
    }

    [When(@"The user sends the currency creation request")]
    public async Task WhenUserSendsTheCurrencyCreationRequest()
    {
        await _currencyApi.Create(_currency);
    }

    [Then("The currency should be saved in the database")]
    public async Task ThenTheCurrencyShouldBeSavedInTheDb()
    {
        var currency = await _mongoDbContext
            .Collection.Find(x => x.CurrencyCode == _currency.CurrencyCode)
            .FirstOrDefaultAsync();
        Assert.That(currency, Is.Not.EqualTo(null));
        Assert.That(currency.CurrencyCode, Is.EqualTo(_currency.CurrencyCode));
    }
}
