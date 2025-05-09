using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using NSubstitute.ReturnsExtensions;
using Refit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ExportPro.StorageService.IntegrationTests.Steps.CurrencySteps;

[Binding]
[Scope(Tag = "CreateCurrency")]
public class CreateCurrencySteps
{
    private readonly IMongoDbContext<Currency> _mongoDbContext = new MongoDbContext<Currency>();
    private CurrencyDto _currency;
    private ICurrencyApi _currencyApi;

    [Given(@"The user is logged in with email '(.*)' and password '(.*)' and has necessary permissions")]
    public async Task GivenTheUserHasValidToken(string email, string password)
    {
        string jwtToken = await UserLogin.Login(email, password);
        HttpClient httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    }

    [Given(@"The user has a currency")]
    public void GivenTheUserHasCurrency(Table table)
    {
        _currency = new CurrencyDto() { CurrencyCode = table.Rows[0]["CurrencyCode"] };
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
