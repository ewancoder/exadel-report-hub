using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Configs;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Refit;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests.Steps.CurrencySteps;

[Binding]
[Scope(Tag = "CurrencyManagement")]
public class CreateCurrencySteps(FeatureContext featureContext)
{
    private static readonly IMongoDbContext<Currency> _mongoDbContext = new MongoDbContext<Currency>();
    private static Guid _currencyId;
    private CurrencyDto? _currency;
    private readonly IConfiguration _config = LoadingConfig.LoadConfig();
    private ICurrencyApi? _currencyApi;

    [Given(@"The '(.*)' user is logged in with email and password and has necessary permissions")]
    public async Task GivenTheUserHasValidToken(string Role)
    {
        var jwtToken = await UserLogin.Login(
            _config.GetSection($"Users:{Role}:Email").Value!,
            _config.GetSection($"Users:{Role}:Password").Value!
        );
        featureContext["JwtToken"] = jwtToken;
        var httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    }

    [Given(@"The user has following currency '(.*)'")]
    public void GivenTheUserHasCurrency(string currencyCode)
    {
        _currency = new CurrencyDto { CurrencyCode = currencyCode };
    }

    [When(@"The user sends the currency creation request")]
    public async Task WhenUserSendsTheCurrencyCreationRequest()
    {
        var currency = await _currencyApi!.Create(_currency!);
        featureContext["CurrencyId"] = currency.Data!.Id;
        Console.WriteLine($"saving {featureContext["CurrencyId"]}");
        _currencyId = currency.Data.Id;
    }

    [Then("The currency should be saved in the database")]
    public async Task ThenTheCurrencyShouldBeSavedInTheDb()
    {
        Console.WriteLine($"should be saved {featureContext["CurrencyId"]}");
        var currency = await _mongoDbContext
            .Collection.Find(x => x.CurrencyCode == _currency!.CurrencyCode)
            .FirstOrDefaultAsync();
        Assert.That(currency, Is.Not.EqualTo(null));
        Assert.That(currency.CurrencyCode, Is.EqualTo(_currency!.CurrencyCode));
    }

    [When(@"The user sends a delete request with currency id")]
    public async Task WhenTheUserSendsCurrencyDeleteRequest()
    {
        Console.WriteLine($"should be deleted in delete {featureContext["CurrencyId"]}");
        var currencyid = (Guid)featureContext["CurrencyId"];
        await _currencyApi!.Delete(currencyid);
    }

    [Then("The currency should be deleted")]
    public async Task ThenTheCurrencyShouldBeDeleted()
    {
        var currency = await _mongoDbContext
            .Collection.Find(x => x.Id == _currencyId.ToObjectId())
            .FirstOrDefaultAsync();
        Assert.That(currency, Is.Not.EqualTo(null));
        Assert.That(currency.IsDeleted, Is.EqualTo(true));
    }

    [AfterFeature("@CurrencyManagement")]
    public static async Task CleanUp()
    {
        await _mongoDbContext.Collection.DeleteOneAsync(x => x.Id == _currencyId.ToObjectId());
    }
}
