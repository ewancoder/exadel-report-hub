using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using NSubstitute.ReturnsExtensions;
using Refit;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests.Steps.CurrencySteps;

[Binding]
public class CreateCurrency
{
    private IMongoDbContext<Currency>? _mongoDbContext = new MongoDbContext<Currency>();
    private string _currencyCode;
    private ICurrencyApi _currencyApi;

    [Given(@"The user have a currency")]
    public void GivenTheUserHaveCurrency()
    {
        _currencyCode = "test";
    }

    [Given("The user has a valid token")]
    public async Task GivenTheUserHasValidToken()
    {
        string jwtToken = await UserLogin.Login("OwnerUserTest@gmail.com", "OwnerUserTest2@");
        HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:1500") };
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    }

    [When(@"The user sends the currency creation request")]
    public async Task WhenUserSendsTheCurrencyCreationRequest()
    {
        await _currencyApi.Create(_currencyCode);
    }

    [Then("The currency should be saved in the database")]
    public async Task ThenTheCurrencyShouldBeSavedInTheDb()
    {
        var currency = await _mongoDbContext
            .Collection.Find(x => x.CurrencyCode == _currencyCode)
            .FirstOrDefaultAsync();
        Assert.That(currency, Is.Not.EqualTo(null));
        Assert.That(currency.CurrencyCode, Is.EqualTo(_currencyCode));
    }

    [AfterScenario]
    public void CleanUp()
    {
        var cleanup = _mongoDbContext
            .Collection.Find(x => x.CurrencyCode == "test")
            .FirstOrDefaultAsync()
            .GetAwaiter()
            .GetResult();
        _mongoDbContext.Collection.DeleteOne(x => x.Id == cleanup.Id);
    }
}
