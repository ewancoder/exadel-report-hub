using ExportPro.Shared.IntegrationTests.Auth;
using TechTalk.SpecFlow;

namespace ExportPro.Shared.IntegrationTests;

[Binding]
public static class AddingTestUsers
{
    [BeforeTestRun]
    public async static Task AddingUsers()
    {
        await UserActions.AddSuperAdmin();
        await UserActions.AddOperator();
        await UserActions.AddOwner();
        await UserActions.AddClientAdmin();
    }
}
