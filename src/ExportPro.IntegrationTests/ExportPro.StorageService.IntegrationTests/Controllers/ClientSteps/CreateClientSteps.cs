using ExportPro.StorageService.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests.Controllers.ClientSteps;

[TestFixture]
[Binding]
public class CreateClientSteps
{
    [SetUp]
    public void Setup()
    {
        IMongoDbContext<Client> mongoDbContext = new MongoDbContext<Client>();
    }
}
