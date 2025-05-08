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
public class CreateCurrency
{
    private readonly IMongoDbContext<Currency> _mongoDbContext = new MongoDbContext<Currency>();
    private CurrencyDto _currency;
    private ICurrencyApi _currencyApi;

    [Given(@"The user have a currency")]
    public void GivenTheUserHaveCurrency()
    {
        _currency = new() { CurrencyCode = "DDD" };
    }

    [Given("The user has a valid token")]
    public void GivenTheUserHasValidToken()
    {
        string jwtToken = UserLogin.Login("OwnerUserTest@gmail.com", "OwnerUserTest2@").GetAwaiter().GetResult();
        HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:1500") };
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    }

    [When(@"The user sends the currency creation request")]
    public void WhenUserSendsTheCurrencyCreationRequest()
    {
        _currencyApi.Create(_currency).GetAwaiter().GetResult();
    }

    [Then("The currency should be saved in the database")]
    public void ThenTheCurrencyShouldBeSavedInTheDb()
    {
        var currency = _mongoDbContext
            .Collection.Find(x => x.CurrencyCode == _currency.CurrencyCode)
            .FirstOrDefaultAsync()
            .GetAwaiter()
            .GetResult();
        Assert.That(currency, Is.Not.EqualTo(null));
        Assert.That(currency.CurrencyCode, Is.EqualTo(_currency.CurrencyCode));
    }
}
