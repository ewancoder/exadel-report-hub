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

namespace ExportPro.StorageService.IntegrationTests.Steps.CurrencySteps;

[Binding]
public class DeleteCurrencySteps
{
    private readonly IMongoDbContext<Currency> _mongoDbContext = new MongoDbContext<Currency>();
    private ICurrencyApi _currencyApi;
    private Guid _currencyId;

    [Given(@"The user has a valid token for deleting")]
    public async Task GivenTheUserHasValidToken()
    {
        string jwtToken = await UserLogin.Login("OwnerUserTest@gmail.com", "OwnerUserTest2@");
        HttpClient httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
    }

    [Given(@"The user has currency id")]
    public async Task GivenTheUserHasCurrencyId()
    {
        CurrencyDto currencyDto = new() { CurrencyCode = "ZZZ" };
        var currency = await _currencyApi.Create(currencyDto);
        _currencyId = currency.Data.Id;
    }

    [When("The user sends the currency delete request")]
    public async Task WhenTheUserSendsCurrencyDeleteRequest()
    {
        await _currencyApi.Delete(_currencyId);
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
}
