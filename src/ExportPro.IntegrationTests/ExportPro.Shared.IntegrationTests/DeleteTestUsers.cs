using ExportPro.Shared.IntegrationTests.Auth;
using TechTalk.SpecFlow;

namespace ExportPro.Shared.IntegrationTests;

[Binding]
public static class DeleteTestUsers
{
    [AfterTestRun]
    public async static Task DeletingTestUsers()
    {
        await UserActions.RemoveUser("SuperAdminTest");
        await UserActions.RemoveUser("OwnerUserTest");
        await UserActions.RemoveUser("ClientAdminTest");
        await UserActions.RemoveUser("OperatorTest");
    }
}
