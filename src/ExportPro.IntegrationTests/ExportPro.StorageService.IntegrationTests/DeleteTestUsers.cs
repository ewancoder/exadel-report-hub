using ExportPro.Shared.IntegrationTests.Auth;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests;

[Binding]
public static class DeleteTestUsers
{
    [AfterTestRun]
    public static async Task DeletingTestUsers()
    {
        try
        {
            await UserActions.RemoveUser("SuperAdminTest");
            await UserActions.RemoveUser("OwnerUserTest");
            await UserActions.RemoveUser("ClientAdminTest");
            await UserActions.RemoveUser("OperatorTest");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during user deletion: {ex.Message}");
        }
    }
}
