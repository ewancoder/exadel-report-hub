using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Bson;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ExportPro.StorageService.IntegrationTests.Steps.ClientSteps;

[Binding]
public class DeleteClientSteps
{
    private readonly IMongoDbContext<Client> _mongoDbContext = new MongoDbContext<Client>();
    private Guid _clientId;
    private IClientApi? _clientApi;

    [Given("The user is logged in with email  and password  and has necessary permissions")]
    public async Task GivenUserHasValidToken(Table table)
    {
        string jwtToken = await UserLogin.Login(table.Rows[0]["Email"], table.Rows[0]["Password"]);
        HttpClient httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _clientApi = RestService.For<IClientApi>(httpClient);
        Assert.That(_clientApi, Is.Not.EqualTo(null));
    }

    [Given("the client exists and The user has a client id")]
    public async Task GivenTheClientExists(Table table)
    {
        ClientDto clientDto = table.CreateInstance<ClientDto>();
        await _clientApi!.CreateClient(clientDto);
        var client = await _mongoDbContext!.Collection.Find(x => x.Name == clientDto.Name).FirstOrDefaultAsync();
        Assert.That(client, Is.Not.Null);
        Assert.That(client.Name, Is.EqualTo(clientDto.Name));
        _clientId = client.Id.ToGuid();
    }

    [When("The user send a delete request")]
    public async Task WhenUserSendsDeleteRequest()
    {
        await _clientApi!.SoftDeleteClient(_clientId);
    }

    [Then("The client should be deleted")]
    public async Task ThenTheClientShouldBeDeleted()
    {
        var client = await _mongoDbContext.Collection.Find(x => x.Id == _clientId.ToObjectId()).FirstOrDefaultAsync();
        Assert.That(client, Is.Not.Null);
        Assert.That(client.IsDeleted, Is.EqualTo(true));
    }
}
