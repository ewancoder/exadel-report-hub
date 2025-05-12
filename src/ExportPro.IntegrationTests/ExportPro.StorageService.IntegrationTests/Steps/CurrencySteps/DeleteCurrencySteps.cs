using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ExportPro.StorageService.IntegrationTests.Steps.CurrencySteps;

[Binding]
[Scope(Tag = "DeleteCurrency")]
public class DeleteCurrencySteps
{
    private readonly IMongoDbContext<Currency> _mongoDbContext = new MongoDbContext<Currency>();
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
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    }

    [Given("the user has created the following currency and stored the currency id")]
    public async Task GivenTheUserHasCreatedTheFollowingCurrencyAndStoredTheCurrencyId(Table table)
    {
        var currencyDto = table.CreateInstance<CurrencyDto>();
        var currency = await _currencyApi!.Create(currencyDto);
        _currencyId = currency.Data!.Id;
    }

    [When("The user sends the currency delete request")]
    public async Task WhenTheUserSendsCurrencyDeleteRequest()
    {
        await _currencyApi!.Delete(_currencyId);
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

    [AfterScenario("@DeleteCurrency")]
    public async Task CleanUp()
    {
        await _mongoDbContext.Collection.DeleteOneAsync(x => x.Id == _currencyId.ToObjectId());
    }
}
