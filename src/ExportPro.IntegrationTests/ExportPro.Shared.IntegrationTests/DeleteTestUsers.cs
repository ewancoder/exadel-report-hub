using ExportPro.Shared.IntegrationTests.Auth;
using TechTalk.SpecFlow;

namespace ExportPro.Shared.IntegrationTests;

[Binding]
public static class DeleteTestUsers
{
    [AfterTestRun]
    public static async Task DeletingTestUsers()
    {
        await UserActions.RemoveUser("SuperAdminTest");
        await UserActions.RemoveUser("OwnerUserTest");
        await UserActions.RemoveUser("ClientAdminTest");
        await UserActions.RemoveUser("OperatorTest");
    }
}
