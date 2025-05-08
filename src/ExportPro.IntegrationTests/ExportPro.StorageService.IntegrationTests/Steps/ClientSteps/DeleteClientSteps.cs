using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Bson;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests.Steps.ClientSteps;

[Binding]
public class DeleteClientSteps
{
    private IMongoDbContext<Client> _mongoDbContext = new MongoDbContext<Client>();
    private Guid _clientId;
    private IStorageServiceApi? _storageServiceApi;

    [Given("User have a valid token")]
    public async Task ValidUserToken()
    {
        string jwtToken = await UserLogin.Login("SuperAdminTest@gmail.com", "SuperAdminTest2@");
        HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:1500") };
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
        _storageServiceApi = RestService.For<IStorageServiceApi>(httpClient);
        Assert.That(_storageServiceApi, Is.Not.EqualTo(null));
    }

    [Given("User have a client id")]
    public async Task ClientId()
    {
        ClientDto clientDto = new() { Name = "DeleteIsI", Description = "Description" };
        await _storageServiceApi.CreateClient(clientDto);
        var client = await _mongoDbContext!.Collection.Find(x => x.Name == "DeleteIsI").FirstOrDefaultAsync();
        _clientId = client.Id.ToGuid();
    }

    [When("User send a delete request")]
    public async Task DeleteRequest()
    {
        await _storageServiceApi!.SoftDeleteClient(_clientId);
    }

    [Then("The client should be deleted")]
    public async Task ShouldBeDeleted()
    {
        var client = await _mongoDbContext.Collection.Find(x => x.Id == _clientId.ToObjectId()).FirstOrDefaultAsync();
        Assert.That(client, Is.Not.Null);
        Assert.That(client.IsDeleted, Is.EqualTo(true));
    }

    [AfterScenario]
    public void Cleanup()
    {
        _mongoDbContext.Collection.DeleteOne(x => x.Id == _clientId.ToObjectId());
    }
}
